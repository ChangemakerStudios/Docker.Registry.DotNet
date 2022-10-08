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

using System;
using System.Collections.Generic;
using System.Linq;

namespace Docker.Registry.DotNet.Helpers
{
    internal class QueryString : IQueryString
    {
        private readonly Dictionary<string, string[]> _values = new Dictionary<string, string[]>();

        public string GetQueryString()
        {
            return string.Join(
                "&",
                this._values.Select(
                    pair => string.Join(
                        "&",
                        pair.Value.Select(
                            v => $"{Uri.EscapeUriString(pair.Key)}={Uri.EscapeDataString(v)}"))));
        }

        public void Add(string key, string value)
        {
            this._values.Add(key, new[] { value });
        }

        public void Add(string key, string[] values)
        {
            this._values.Add(key, values);
        }
    }
}