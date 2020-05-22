/*
 * ObjFile.cs
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

using System.IO;
using System.Text;

namespace CSharpCSG
{
    /// 
    /// 
    /// author: Michael Hoffer &lt;info@michaelhoffer.de&gt;
    /// 
    public sealed class ObjFile
    {

        private string obj;
        private readonly string mtl;
        private Stream objStream;
        private Stream mtlStream;

        public const string MTL_NAME = "$JCSG_MTL_NAME$";

        public ObjFile(string obj, string mtl)
        {
            this.obj = obj;
            this.mtl = mtl;
        }

        public void toFiles(string p)
        {

            string parent = Path.GetDirectoryName(p);

            string fileName = Path.GetFileName(p);

            if (fileName.ToLower().EndsWith(".obj") || fileName.ToLower().EndsWith(".mtl"))
            {
                fileName = fileName.Substring(0, fileName.Length - 4);
            }

            string objName = fileName + ".obj";
            string mtlName = fileName + ".mtl";

            obj = obj.Replace(MTL_NAME, mtlName);
                objStream = null;

            if (parent == null)
            {

                FileUtil.write(objName, obj);
                FileUtil.write(mtlName, mtl);
            }
            else
            {
                
                FileUtil.write(Path.Combine(parent, objName), obj);
                FileUtil.write(Path.Combine(parent, mtlName), mtl);
            }

        }

        public string getObj()
        {
            return this.obj;
        }

        public string getMtl()
        {
            return this.mtl;
        }

        public Stream getObjStream()
        {
            if (objStream == null)
            {
                objStream = new MemoryStream(Encoding.UTF8.GetBytes(obj));
            }

            return objStream;
        }

        public Stream getMtlStream()
        {
            if (mtlStream == null)
            {
                mtlStream = new MemoryStream(Encoding.UTF8.GetBytes(mtl));
            }

            return mtlStream;
        }
    }
}
