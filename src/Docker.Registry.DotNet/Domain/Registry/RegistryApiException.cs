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

namespace Docker.Registry.DotNet.Domain.Registry;

public class RegistryApiException : Exception
{
    internal RegistryApiException(RegistryApiResponse response)
        : base($"Docker API responded with status code={response.StatusCode}")
    {
        StatusCode = response.StatusCode;
        Headers = response.Headers;
    }

    public HttpStatusCode StatusCode { get; }

    public HttpResponseHeaders Headers { get; }
}

public class RegistryApiException<TBody> : RegistryApiException
{
    internal RegistryApiException(RegistryApiResponse<TBody> response)
        : base(response)
    {
        Body = response.Body;
    }

    public TBody? Body { get; }
}