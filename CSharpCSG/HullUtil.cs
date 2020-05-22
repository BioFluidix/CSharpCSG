/*
 * HullUtil.cs
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

using CSharpVecMath;
using QuickHull3D;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpCSG
{
    /// 
    /// 
    /// author: Michael Hoffer &lt;info@michaelhoffer.de&gt;
    /// 
    public class HullUtil
    {

        private HullUtil()
        {
            throw new Exception("Don't instantiate me!", null);
        }

        public static CSG hull(List<IVector3d> points, PropertyStorage storage)
        {

            Point3d[] hullPoints = points.Select(vec => new Point3d(vec.x(), vec.y(), vec.z())).ToArray();

            Hull hull = new Hull();
            hull.Build(hullPoints);
            hull.Triangulate();

            int[][] faces = hull.GetFaces();

            List<Polygon> polygons = new List<Polygon>();

            List<IVector3d> vertices = new List<IVector3d>();

            foreach (int[] verts in faces)
            {

                foreach (int i in verts)
                {
                    vertices.Add(points[hull.GetVertexPointIndices()[i]]);
                }

                polygons.Add(Polygon.fromPoints(vertices, storage));

                vertices.Clear();
            }

            return CSG.fromPolygons(polygons);
        }

        public static CSG hull(CSG csg, PropertyStorage storage)
        {

            List<IVector3d> points = new List<IVector3d>(csg.getPolygons().Count * 3);

            csg.getPolygons().ForEach( p => p.vertices.ForEach( v => points.Add(v.pos)));

            return hull(points, storage);
        }
    }
}
