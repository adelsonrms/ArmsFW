using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ArmsFW.Services.Extensions
{
    public static class LinqExtensions
    {
        public static IQueryable<TSource> Between<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, TKey menor, TKey maior) where TKey : IComparable<TKey>
        {
            Expression key = Expression.Invoke(keySelector, keySelector.Parameters.ToArray());
            Expression lowerBound = Expression.GreaterThanOrEqual(key, Expression.Constant(menor));
            Expression upperBound = Expression.LessThanOrEqual(key, Expression.Constant(maior));
            Expression and = Expression.AndAlso(lowerBound, upperBound);
            Expression<Func<TSource, bool>> lambda = Expression.Lambda<Func<TSource, bool>>(and, keySelector.Parameters);
            return source.Where(lambda);
        }

        #region License and Terms
        // MoreLINQ - Extensions to LINQ to Objects
        // Copyright (c) 2008 Jonathan Skeet. All rights reserved.
        //
        // Licensed under the Apache License, Version 2.0 (the "License");
        // you may not use this file except in compliance with the License.
        // You may obtain a copy of the License at
        //
        //     http://www.apache.org/licenses/LICENSE-2.0
        //
        // Unless required by applicable law or agreed to in writing, software
        // distributed under the License is distributed on an "AS IS" BASIS,
        // WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
        // See the License for the specific language governing permissions and
        // limitations under the License.
        #endregion


        /// <summary>
        /// Returns all distinct elements of the given source, where "distinctness"
        /// is determined via a projection and the default equality comparer for the projected type.
        /// </summary>
        /// <remarks>
        /// This operator uses deferred execution and streams the results, although
        /// a set of already-seen keys is retained. If a key is seen multiple times,
        /// only the first element with that key is returned.
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="keySelector">Projection for determining "distinctness"</param>
        /// <returns>A sequence consisting of distinct elements from the source sequence,
        /// comparing them by the specified key projection.</returns>

        //public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
        //    Func<TSource, TKey> keySelector)
        //{
        //    return source.DistinctBy(keySelector, null);
        //}

        /// <summary>
        /// Returns all distinct elements of the given source, where "distinctness"
        /// is determined via a projection and the specified comparer for the projected type.
        /// </summary>
        /// <remarks>
        /// This operator uses deferred execution and streams the results, although
        /// a set of already-seen keys is retained. If a key is seen multiple times,
        /// only the first element with that key is returned.
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="keySelector">Projection for determining "distinctness"</param>
        /// <param name="comparer">The equality comparer to use to determine whether or not keys are equal.
        /// If null, the default equality comparer for <c>TSource</c> is used.</param>
        /// <returns>A sequence consisting of distinct elements from the source sequence,
        /// comparing them by the specified key projection.</returns>

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            return _(); IEnumerable<TSource> _()
            {
                var knownKeys = new HashSet<TKey>(comparer);
                foreach (var element in source)
                {
                    if (knownKeys.Add(keySelector(element)))
                        yield return element;
                }
            }
        }


    }

    public class ComparadorGenerico<T> : IEqualityComparer<T>
    {
        public Func<T, T, bool> MetodoEquals { get; }
        public Func<T, int> MetodoGetHashCode { get; }
        private ComparadorGenerico(
            Func<T, T, bool> metodoEquals,
            Func<T, int> metodoGetHashCode)
        {
            this.MetodoEquals = metodoEquals;
            this.MetodoGetHashCode = metodoGetHashCode;
        }

        public static ComparadorGenerico<T> Criar(
            Func<T, T, bool> metodoEquals,
            Func<T, int> metodoGetHashCode)
                => new ComparadorGenerico<T>(
                        metodoEquals,
                        metodoGetHashCode
                    );

        public bool Equals(T x, T y)
            => MetodoEquals(x, y);

        public int GetHashCode(T obj)
            => MetodoGetHashCode(obj);
    }

    public static class DistinctExtension
    {
        public static IEnumerable<TSource> DistinctItens<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, TSource, bool> metodoEquals,
            Func<TSource, int> metodoGetHashCode)
                => source.Distinct(
                    ComparadorGenerico<TSource>.Criar(
                        metodoEquals,
                        metodoGetHashCode)
                        );
    }

    public static class ListExtensions
    {
        public static string ToListString<T>(this IEnumerable<T> list, string separador = " \n", string close = "")
        {

            return string.Join(separador, list.Select(s => $"{close}{s}{close}"));
            //string strList = "";

            //list.ToList().ForEach(x => strList += $"{close}{x}{close}{separador}");

            //if (strList.Length>1)
            //{
            //    return strList.Substring(0, strList.Length - 1);
            //}
            //else
            //{
            //    return "";
            //}
        }

        public static string ToListString<T>(this List<T> list, string separador = " \n", string close = "")
        {
            return string.Join(separador, list.Select(s => $"{close}{s}{close}"));

            //string strList = "";

            //((List<T>)list).ForEach(x => strList += $"{close}{x}{close}{separador}");

            //if (strList.Length > 1)
            //{
            //    return strList.Substring(0, strList.Length - 1);
            //}
            //else
            //{
            //    return "";
            //}
        }
        public static T Next<T>(this IList<T> list, T item)
        {
            var nextIndex = list.IndexOf(item) + 1;

            if (nextIndex == list.Count)
            {
                return list[0];
            }

            return list[nextIndex];
        }
    }




}
