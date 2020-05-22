/*
 * PropertyStorage.cs
 *
 * Copyright 2014-2017 Michael Hoffer <info@michaelhoffer.de>. All rights
 * reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice,
 * this list of conditions and the following disclaimer.
 *
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 * this list of conditions and the following disclaimer in the documentation
 * and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY Michael Hoffer <info@michaelhoffer.de> "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL Michael Hoffer <info@michaelhoffer.de> OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
 * OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
 * OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 * The views and conclusions contained in the software and documentation are
 * those of the authors and should not be interpreted as representing official
 * policies, either expressed or implied, of Michael Hoffer
 * <info@michaelhoffer.de>.
 */

using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace CSharpCSG
{
    /// <summary>
    /// A simple property storage.
    /// </summary>
    /// 
    /// author: Michael Hoffer &lt;info@michaelhoffer.de&gt;
    /// 
    public class PropertyStorage
    {

        private readonly Dictionary<string, object> map = new Dictionary<string, object>();

        private static readonly Color[] colors = new Color[]
        {
            Colors.Red, Colors.Yellow, Colors.Green, Colors.Blue, Colors.Magenta,
            Colors.White, Colors.Black, Colors.Gray, Colors.Orange
        };

        /// <summary>
        /// Constructor. Creates a new property storage.
        /// </summary>
        public PropertyStorage()
        {
            randomColor(this);
        }

        /// <summary>
        /// Sets a property. Existing properties are overwritten.
        /// </summary>
        /// 
        /// <param name="key">key</param>
        /// <param name="property">property</param>
        /// 
        public void set(string key, object property)
        {
            map[key] = property;
        }

        /// <summary>
        /// Returns a property.
        /// </summary>
        /// 
        /// <param name="<T>">property type</param>
        /// <param name="key">key</param>
        /// <returns>the property; an empty <see cref="java.util.Optional"/> will be</returns>
        /// returned if the property does not exist or the type does not match
        /// 
        public T getValue<T>(string key)
        {
            object value;

            if (map.TryGetValue(key, out value))
            {
                return (T)((value is T) ? value : null);
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Deletes the requested property if present. Does nothing otherwise.
        /// </summary>
        /// 
        /// <param name="key">key</param>
        /// 
        public void delete(String key)
        {
            map.Remove(key);
        }

        /// <summary>
        /// Indicates whether this storage contains the requested property.
        /// </summary>
        /// 
        /// <param name="key">key</param>
        /// <returns><c>true</c> if this storage contains the requested property;</returns>
        /// <c>false</c>
        /// 
        public bool contains(String key)
        {
            return map.ContainsKey(key);
        }

        static void randomColor(PropertyStorage storage)
        {
            var r = new Random();
            Color c = colors[(int)(r.NextDouble() * colors.Length)];

            storage.set("material:color", $"{c.R} {c.G} {c.B}");
        }
    }
}
