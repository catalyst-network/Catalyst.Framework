#region LICENSE

/**
* Copyright (c) 2019 Catalyst Network
*
* This file is part of Catalyst.Node <https://github.com/catalyst-network/Catalyst.Node>
*
* Catalyst.Node is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 2 of the License, or
* (at your option) any later version.
*
* Catalyst.Node is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with Catalyst.Node. If not, see <https://www.gnu.org/licenses/>.
*/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Dawn;

namespace Catalyst.Common.Extensions
{
    public static class RandomElementExtension
    {
        private static readonly Random Rng = new Random();

        public static T RandomElement<T>(this IEnumerable<T> list)
        {
            var value = list.ToList();
            Guard.Argument(value).MinCount(1);
            var enumerable = list as T[] ?? value.ToArray();
            return enumerable[Rng.Next(enumerable.Length)];
        }

        public static IList<T> Shuffle<T>(this IEnumerable<T> source)
        {
            Guard.Argument(source, nameof(source)).NotNull();
            var list = source as List<T> ?? source.ToList();

            var randomlyMapped = Enumerable.Range(0, list.Count)
               .Select(i => new {Index = i, SortingKey = Rng.Next()})
               .OrderBy(z => z.SortingKey)
               .Select(z => list[z.Index])
               .ToList();

            return randomlyMapped;
        }
    }
}
