// Copyright 2017-2024 Rich Quackenbush, Jaben Cargman
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

using System.Diagnostics.CodeAnalysis;

namespace Docker.Registry.DotNet.Domain.ImageReferences;

public record ImageTag
{
    public ImageTag(string value)
    {
        var parsedTag = value?.Trim() ?? string.Empty;

        var errors = ValidateTag(parsedTag).ToList();

        if (errors.Any())
        {
            throw new ArgumentException($"Invalid Image Tag: {errors.ToDelimitedString(", ")}", nameof(value));
        }

        this.Value = parsedTag;
    }

    public string Value { get; init; }

    public static ImageTag Latest { get; } = new("latest");

    public ImageReference ToReference() => new(this);

    public void Deconstruct(out string value)
    {
        value = this.Value;
    }

    public static ImageTag Create(string tag) => new(tag);

    public static bool TryCreate(string reference, [NotNullWhen(true)] out ImageTag? imageTag)
    {
        reference = reference?.Trim() ?? string.Empty;

        var errors = ValidateTag(reference).ToList();

        imageTag = null;

        if (errors.Any())
        {
            return false;
        }

        imageTag = new ImageTag(reference);

        return true;
    }

    static IEnumerable<string> ValidateTag(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            yield return "Value must not be null or empty";

            yield break;
        }

        if (value.Length > 128)
        {
            yield return $"Value is too large. Value is {value.Length
            } characters and maximum tag size is 128 characters.";

            yield break;
        }

        var validChars = new[] { '-', '_', '.' };

        var invalidCharacters =
#if NET7_0_OR_GREATER
            value.Where(c => !char.IsAsciiLetterOrDigit(c) && !validChars.Contains(c)).ToArray();
#else
            value.Where(c => !char.IsLetterOrDigit(c) && !validChars.Contains(c)).ToArray();
#endif

        if (invalidCharacters.Any())
        {
            yield return @$"Value ""{value}"" is invalid characters: ""{
                invalidCharacters.Select(s => $"{s}").ToDelimitedString(",")
            }"". Image Tags can only contain lowercase and uppercase letters, digits, underscores, periods, and hyphens.";

            yield break;
        }

        if (value.StartsWith(".") || value.StartsWith("-"))
        {
            yield return @$"Value ""{value}"" is invalid. Image References can't start with a period or hyphen.";
        }
    }

    public override string ToString() => this.Value;
}
