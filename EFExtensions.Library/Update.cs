using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EFExtensions.Library
{
    public static class Update
    {
        /// <summary>
        /// Insert or Update into table from a list of objects
        /// </summary>
        /// <typeparam name="T">Type of Entity</typeparam>
        /// <param name="db">The Database context</param>
        /// <param name="values">The values to be inserted</param>
        /// <returns>Number of rows affected</returns>
        public static int InsertOrUpdate<T>(this DbContext db, List<T> values)
            where T : class
        {
            foreach (var item in values)
            {
                db.PrimaryKeys(item, out EntityKey entityKeys);
                var isupdate = (db as IObjectContextAdapter).ObjectContext.TryGetObjectByKey(entityKeys, out object _);

                MethodInfo getprop = typeof(Update).GetMethod("GetPropertyValue");

                if (isupdate)
                {
                    T b = db.Set<T>().Find(entityKeys.EntityKeyValues.Select(s => s.Value).ToArray());

                    item.GetType().GetProperties()
                        .Where(s => !entityKeys.EntityKeyValues.Any(e => e.Key == s.Name))
                        .ToList().ForEach(f =>
                        {
                            var srcval = getprop.MakeGenericMethod(f.PropertyType).Invoke(null, new object[] { item, f.Name });

                            b.SetPropertyValue(f.Name, srcval);
                        });
                }
                else
                {
                    db.Set<T>().Add(item);
                }
            }

            return db.SaveChanges();
        }

        /// <summary>
        /// Insert or Update a single object into table
        /// </summary>
        /// <typeparam name="T">Type of Entity</typeparam>
        /// <param name="db">The Database context</param>
        /// <param name="dataexpression">The expression representing data to be inserted</param>
        /// <returns>Number of rows affected</returns>
        public static int InsertOrUpdate<T>(this DbContext db, Expression<Func<T, T>> dataexpression)
            where T : class
        {
            T newobject = (T)Convert.ChangeType(dataexpression.Compile().DynamicInvoke(Activator.CreateInstance<T>()), typeof(T));
            db.PrimaryKeys(newobject, out EntityKey entityKeys);

            var isupdate = (db as IObjectContextAdapter).ObjectContext.TryGetObjectByKey(entityKeys, out object _);

            MethodInfo getprop = typeof(Update).GetMethod("GetPropertyValue");

            if (isupdate)
            {
                T b = db.Set<T>().Find(entityKeys.EntityKeyValues.Select(s => s.Value).ToArray());

                (dataexpression.Body as MemberInitExpression).Bindings
                    .Where(s => !entityKeys.EntityKeyValues.Any(e => e.Key == s.Member.Name))
                    .ToList().ForEach(f =>
                    {
                        PropertyInfo pi = f.Member as PropertyInfo;

                        var srcval = getprop.MakeGenericMethod(pi.PropertyType).Invoke(null, new object[] { newobject, pi.Name });

                        b.SetPropertyValue(pi.Name, srcval);
                    });
            }
            else
            {
                db.Set<T>().Add(newobject);
            }

            return db.SaveChanges();
        }

        /// <summary>
        /// Update the entry in database identified by query parameter
        /// </summary>
        /// <typeparam name="T">Type of Entity</typeparam>
        /// <param name="db">The Database context</param>
        /// <param name="query">The filter expression used for identifying the entry</param>
        /// <param name="dataexpression">The new value assignments</param>
        /// <returns>Number of rows affected</returns>
        public static int UpdateFromQuery<T>(this DbContext db, Expression<Func<T, bool>> query, Expression<Func<T, T>> dataexpression)
            where T : class
        {
            if (!db.Set<T>().Any(query))
                return default;

            var existingrecords = db.Set<T>().Where(query).ToList();
            T newobject = (T)Convert.ChangeType(dataexpression.Compile().DynamicInvoke(Activator.CreateInstance<T>()), typeof(T));

            bool iskeyupdate = false;

            List<string> keys = db.PrimaryKeys(newobject, out EntityKey _);

            if (keys.Any())
            {
                iskeyupdate = (dataexpression.Body as MemberInitExpression).Bindings.Any(a => keys.Contains(a.Member.Name));
            }

            List<T> records;

            if (iskeyupdate)
                records = existingrecords.Select(s => s.DeepCopy()).ToList();
            else
                records = existingrecords;

            MethodInfo getprop = typeof(ReflectionExtensions).GetMethod("GetPropertyValue");

            foreach (var binding in (dataexpression.Body as MemberInitExpression).Bindings)
            {
                PropertyInfo pi = binding.Member as PropertyInfo;

                var srcval = getprop.MakeGenericMethod(pi.PropertyType).Invoke(null, new object[] { newobject, pi.Name });

                records.ForEach(a => a.SetPropertyValue(pi.Name, srcval));
            }

            if (iskeyupdate)
            {
                db.Set<T>().RemoveRange(existingrecords);
                db.Set<T>().AddRange(records);
            }

            return db.SaveChanges();
        }

        /// <summary>
        /// Update the prefiltered entry in database
        /// </summary>
        /// <typeparam name="T">Type of Entity</typeparam>
        /// <param name="values">The IQueryable collection of filtered entries</param>
        /// <param name="value">The new value assignment</param>
        public static void UpdateFromQuery<T>(this IQueryable<T> values, Action<T> assign)
            where T : class
        {
            values.ForEachAsync(assign).Wait();
        }

        /// <summary>
        /// Retrive the primary keys for a given Entity in the provided Database context
        /// </summary>
        /// <typeparam name="T">Type of database entities</typeparam>
        /// <param name="db">The Database context</param>
        /// <param name="entity">The entity for which primary key information is requested</param>
        /// <param name="entityKey">The EntityKey object</param>
        /// <returns>List of key names</returns>
        public static List<string> PrimaryKeys<T>(this DbContext db, T entity, out EntityKey entityKey)
            where T : class
        {
            var objContext = (db as IObjectContextAdapter).ObjectContext;
            var objectset = objContext.CreateObjectSet<T>();
            var keys = objectset.EntitySet.ElementType.KeyMembers.Select(s => s.Name).ToList();

            try
            {
                entityKey = objContext.CreateEntityKey(objectset.EntitySet.Name, entity);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                entityKey = null;
            }

            return keys;
        }
    }
}
