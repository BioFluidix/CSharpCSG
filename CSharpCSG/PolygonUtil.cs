/*
 * PolygonUtil.cs
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
using System;
using System.Collections.Generic;

namespace CSharpCSG
{
    /// <summary>
    /// PolygonUtil
    /// </summary>
    /// 
    /// author: Michael Hoffer &lt;info@michaelhoffer.de&gt;
    /// 
    public class PolygonUtil
    {

        private PolygonUtil()
        {
            throw new Exception("Don't instantiate me!", null);
        }

        /// <summary>
        /// Converts a CSG polygon to a poly2tri polygon (including holes)
        /// </summary>
        /// <param name="polygon">the polygon to convert</param>
        /// <returns>a CSG polygon to a poly2tri polygon (including holes)</returns>
        /// 
        private static Poly2Tri.Polygon fromCSGPolygon(Polygon polygon)
        {

            // convert polygon
            List<Poly2Tri.PolygonPoint> points = new List<Poly2Tri.PolygonPoint>();
            foreach (Vertex v in polygon.vertices)
            {
                Poly2Tri.PolygonPoint vp = new Poly2Tri.PolygonPoint(v.pos.x(), v.pos.y(), v.pos.z());
                points.Add(vp);
            }

            Poly2Tri.Polygon result = new Poly2Tri.Polygon(points);

            // convert holes
            List<Polygon> holesOfPresult = polygon.getStorage().getValue<List<Polygon>>(Edge.KEY_POLYGON_HOLES);
            if (holesOfPresult != null)
            {
                List<Polygon> holesOfP = holesOfPresult;

                holesOfP.ForEach(hP => result.addHole(fromCSGPolygon(hP)));
            }

            return result;
        }

        public static List<Polygon> concaveToConvex(Polygon concave)
        {

            List<Polygon> result = new List<Polygon>();

            IVector3d normal = concave.vertices[0].normal.clone();

            bool cw = !Extrude.isCCW(concave);

            Poly2Tri.Polygon p = fromCSGPolygon(concave);

            Poly2Tri.Poly2Tri.triangulate(p);

            List<Poly2Tri.DelaunayTriangle> triangles = p.getTriangles();

            List<Vertex> triPoints = new List<Vertex>();

            foreach (Poly2Tri.DelaunayTriangle t in triangles)
            {

                int counter = 0;
                foreach (Poly2Tri.TriangulationPoint tp in t.points)
                {

                    triPoints.Add(new Vertex(
                            Vector3d.xyz(tp.getX(), tp.getY(), tp.getZ()),
                            normal));

                    if (counter == 2)
                    {
                        if (!cw)
                        {
                            triPoints.Reverse();
                        }
                        Polygon poly =
                                new Polygon(
                                        triPoints, concave.getStorage());
                        result.Add(poly);
                        counter = 0;
                        triPoints = new List<Vertex>();

                    }
                    else
                    {
                        counter++;
                    }
                }
            }

            return result;
        }
    }
}
