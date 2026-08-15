//  Copyright 2017-2022 Rich Quackenbush, Jaben Cargman
//  and Docker.Registry.DotNet Contributors
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

namespace Docker.Registry.DotNet.Infrastructure.Helpers;

internal static class DictionaryExtensions
{
    public static string GetQueryString(this IDictionary<string, string[]> values)
    {
        return string.Join(
            "&",
            values.Select(
                pair => string.Join(
                    "&",
                    pair.Value.Select(
                        v => $"{Uri.EscapeUriString(pair.Key)}={Uri.EscapeDataString(v)}"))));
    }

    public static TValue? GetValueOrDefault<TKey, TValue>(
        this IDictionary<TKey, TValue> dict,
        TKey key)
    {
        return dict.TryGetValue(key, out var value) ? value : default;
    }
}