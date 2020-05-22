/*
 * STL.cs
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

using CSharpSTL;
using CSharpVecMath;
using System.Collections.Generic;

namespace CSharpCSG
{
    /// <summary>
    /// Loads a CSG from stl.
    /// </summary>
    /// 
    /// author: Michael Hoffer &lt;info@michaelhoffer.de&gt;
    /// 
    public class STL
    {
        ///
        /// Loads a CSG from stl.
        /// 
        /// <param name="path">file path</param>
        /// <returns>CSG</returns>
        /// @throws IOException if loading failed
        ///
        public static CSG file(string path)
        {
            var solid = STLSolid.CreateFromFile(path);

            List<Polygon> polygons = new List<Polygon>();
            List<IVector3d> vertices = new List<IVector3d>();
            foreach(var facet in solid.Facets)
            {
                vertices.Add(Vector3d.xyz(facet.OuterLoop.V0.X, facet.OuterLoop.V0.Y, facet.OuterLoop.V0.Z));
                vertices.Add(Vector3d.xyz(facet.OuterLoop.V1.X, facet.OuterLoop.V1.Y, facet.OuterLoop.V1.Z));
                vertices.Add(Vector3d.xyz(facet.OuterLoop.V2.X, facet.OuterLoop.V2.Y, facet.OuterLoop.V2.Z));
                if (vertices.Count == 3)
                {
                    polygons.Add(Polygon.fromPoints(vertices));
                    vertices = new List<IVector3d>();
                }
            }

            //foreach (IVector3d p in loader.parse(path))
            //{
            //    vertices.Add(p.clone());
            //    if (vertices.Count == 3)
            //    {
            //        polygons.Add(Polygon.fromPoints(vertices));
            //        vertices = new List<IVector3d>();
            //    }
            //}

            return CSG.fromPolygons(new PropertyStorage(), polygons);
        }
    }
}
