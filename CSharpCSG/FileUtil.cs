/*
 * FileUtil.cs
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
using System.IO;
using System.Text;

namespace CSharpCSG
{
    /// <summary>
    /// File util class.
    /// </summary>
    /// 
    /// author: Michael Hoffer &lt;info@michaelhoffer.de&gt;
    /// 
    public class FileUtil
    {

        private FileUtil()
        {
            throw new Exception("Don't instantiate me", null);
        }

        /// <summary>
        /// Writes the specified string to a file.
        /// </summary>
        /// <param name="p">file destination (existing files will be overwritten)</param>
        /// <param name="s">string to save</param>
        /// 
        /// <exception cref="IOException">if writing to file fails</exception>
        /// 
        public static void write(string p, string s)
        {
            using (var file = File.Open(p, FileMode.Create | FileMode.Truncate))
            {
                using (var writer = new StreamWriter(file, Encoding.UTF8))
                {
                    writer.Write(s);
                }
            }

        }

        /// <summary>
        /// Reads the specified file to a string.
        /// </summary>
        /// 
        /// <param name="p">file to read</param>
        /// <returns>the content of the file</returns>
        /// 
        /// <exception cref="IOException">if reading from file failed</exception>
        /// 
        public static string read(string p)
        {
            string res = null;
            using (var file = File.Open(p, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(file, Encoding.UTF8))
                {
                    res = reader.ReadToEnd();
                }
            }
            return res;
        }


        /// <summary>
        /// Saves the specified csg using STL ASCII format.
        /// </summary>
        /// <param name="path">destination path</param>
        /// <param name="csg">csg to save</param>
        /// <exception cref="IOException"></exception>
        /// 
        public static void toStlFile(string p, CSG csg)
        {

            using (var file = File.Open(p, FileMode.Create | FileMode.Truncate))
            {
                using (var writer = new StreamWriter(file, Encoding.UTF8))
                {
                    writer.Write("solid v3d.csg\n");
                    csg.getPolygons().ForEach(poly => writer.Write(poly.toStlString()));
                    writer.Write("endsolid v3d.csg\n");
                }
            }
        }
    }
}
