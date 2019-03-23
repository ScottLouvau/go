// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

using go.Components;

namespace go.Search
{
    public enum CharacterType : byte
    {
        Letter = 1,
        Digit = 2,
        Other = 3
    }

    /// <summary>
    ///  Word represents a portion of a string for which each character is the same type.
    /// </summary>
    public struct Word
    {
        public int Index { get; set; }
        public int Length { get; set; }
        public CharacterType Type { get; set; }

        public string ToString(string valueContainingWord)
        {
            return valueContainingWord.Substring(this.Index, this.Length);
        }
    }

    public static class WordSplitter
    {
        /// <summary>
        ///  Split a string into words. 
        ///  Each word is only letters, numbers, or other character types.
        /// </summary>
        /// <param name="value">Value to split</param>
        /// <param name="reuse">Previous Split return value to reuse</param>
        /// <returns>Words within value</returns>
        public static PartialArray<Word> Split(string value, PartialArray<Word> reuse = null)
        {
            PartialArray<Word> result = reuse ?? new PartialArray<Word>();
            result.Clear();

            if (String.IsNullOrEmpty(value)) { return result; }

            Word current = new Word() { Index = 0, Type = Type(value[0]) };

            for (int i = 1; i < value.Length; ++i)
            {
                CharacterType typeHere = Type(value[i]);

                // Add a word each time the letter category changes
                if (typeHere != current.Type)
                {
                    current.Length = (i - current.Index);
                    result.Add(current);

                    current = new Word() { Index = i, Type = typeHere };
                }
            }

            current.Length = (value.Length - current.Index);
            result.Add(current);

            return result;
        }

        public static CharacterType Type(char c)
        {
            if (Char.IsLetter(c)) { return CharacterType.Letter; }
            if (Char.IsDigit(c)) { return CharacterType.Digit; }
            return CharacterType.Other;
        }
    }
}
