/*
 * Extrude.cs
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
using System.Linq;

namespace CSharpCSG
{
    /// <summary>
    /// Extrudes concave and convex polygons.
    /// </summary>
    /// author: Michael Hoffer &lt;info@michaelhoffer.de&gt;
    /// 
    public class Extrude
    {

        private Extrude()
        {
            throw new Exception("Don't instantiate me!", null);
        }

        /// <summary>
        /// Extrudes the specified path (convex or concave polygon without holes or
        /// intersections, specified in CCW) into the specified direction.
        /// </summary>
        /// <param name="dir">direction</param>
        /// <param name="points">path (convex or concave polygon without holes or
        /// intersections)</param>
        /// 
        /// <returns>a CSG object that consists of the extruded polygon</returns>
        /// 
        public static CSG points(IVector3d dir, params IVector3d[] points)
        {

            return extrude(dir, Polygon.fromPoints(toCCW(points.ToList())));
        }

        /// </summary>
        /// Extrudes the specified path (convex or concave polygon without holes or
        /// intersections, specified in CCW) into the specified direction.
        /// </summary>
        /// <param name="dir">direction</param>
        /// <param name="points">path (convex or concave polygon without holes or
        /// intersections)</param>
        /// 
        /// <returns>a CSG object that consists of the extruded polygon</returns>
        /// 
        public static CSG points(IVector3d dir, List<IVector3d> points)
        {

            List<IVector3d> newList = new List<IVector3d>(points);

            return extrude(dir, Polygon.fromPoints(toCCW(newList)));
        }

        /// <summary>
        /// Extrudes the specified path (convex or concave polygon without holes or
        /// intersections, specified in CCW) into the specified direction.
        /// </summary>
        /// <param name="dir">direction</param>
        /// <param name="points">path (convex or concave polygon without holes or
        /// intersections)</param>
        /// 
        /// <returns>a list containing the extruded polygon</returns>
        /// 
        public static List<Polygon> points(IVector3d dir, bool top, bool bottom, params IVector3d[] points)
        {

            return extrude(dir, Polygon.fromPoints(toCCW(points.ToList())), top, bottom);
        }

        /// <summary>
        /// Extrudes the specified path (convex or concave polygon without holes or
        /// intersections, specified in CCW) into the specified direction.
        /// </summary>
        /// <param name="dir">direction</param>
        /// <param name="points1">path (convex or concave polygon without holes or
        /// intersections)</param>
        /// <param name="points1">path (convex or concave polygon without holes or
        /// intersections)</param>
        /// 
        /// <returns>a list containing the extruded polygon</returns>
        /// 
        public static List<Polygon> points(IVector3d dir, bool top, bool bottom, List<IVector3d> points1)
        {

            List<IVector3d> newList1 = new List<IVector3d>(points1);

            return extrude(dir, Polygon.fromPoints(toCCW(newList1)), top, bottom);
        }

        /// <summary>
        /// Combines two polygons into one CSG object. Polygons p1 and p2 are treated as top and
        /// bottom of a tube segment with p1 and p2 as the profile. <b>Note:</b> both polygons must have the
        /// same number of vertices. This method does not guarantee intersection-free CSGs. It is in the
        /// responsibility of the caller to ensure that the orientation of p1 and p2 allow for
        /// intersection-free combination of both.
        /// </summary>
        /// <param name="p1">first polygon</param>
        /// <param name="p2">second polygon</param>
        /// <returns>List of polygons</returns>
        /// 
        public static CSG combine(Polygon p1, Polygon p2)
        {
            return CSG.fromPolygons(combine(p1, p2, true, true));
        }

        /// <summary>
        /// Combines two polygons into one CSG object. Polygons p1 and p2 are treated as top and
        /// bottom of a tube segment with p1 and p2 as the profile. <b>Note:</b> both polygons must have the
        /// same number of vertices. This method does not guarantee intersection-free CSGs. It is in the
        /// responsibility of the caller to ensure that the orientation of p1 and p2 allow for
        /// intersection-free combination of both.
        /// </summary>
        /// <param name="p1">first polygon</param>
        /// <param name="p2">second polygon</param>
        /// <param name="bottom">defines whether to close the bottom of the tube</param>
        /// <param name="top">defines whether to close the top of the tube</param>
        /// <returns>List of polygons</returns>
        /// 
        public static List<Polygon> combine(Polygon p1, Polygon p2, bool bottom, bool top)
        {
            List<Polygon> newPolygons = new List<Polygon>();

            if (p1.vertices.Count != p2.vertices.Count)
            {
                throw new Exception("Polygons must have the same number of vertices");
            }

            int numVertices = p1.vertices.Count;

            if (bottom)
            {
                newPolygons.Add(p1.flipped());
            }

            for (int i = 0; i < numVertices; i++)
            {

                int nexti = (i + 1) % numVertices;

                IVector3d bottomV1 = p1.vertices[i].pos;
                IVector3d topV1 = p2.vertices[i].pos;
                IVector3d bottomV2 = p1.vertices[nexti].pos;
                IVector3d topV2 = p2.vertices[nexti].pos;

                List<IVector3d> pPoints;

                pPoints = new List<IVector3d> { bottomV2, topV2, topV1 };
                newPolygons.Add(Polygon.fromPoints(pPoints, p1.getStorage()));
                pPoints = new List<IVector3d> { bottomV2, topV1, bottomV1 };
                newPolygons.Add(Polygon.fromPoints(pPoints, p1.getStorage()));
            }

            if (top)
            {
                newPolygons.Add(p2);
            }

            return newPolygons;
        }

        private static CSG extrude(IVector3d dir, Polygon polygon1)
        {
            List<Polygon> newPolygons = new List<Polygon>();

            if (dir.z() < 0)
            {
                throw new ArgumentException("z < 0 currently not supported for extrude: " + dir);
            }

            newPolygons.AddRange(PolygonUtil.concaveToConvex(polygon1));
            Polygon polygon2 = polygon1.translated(dir);

            int numvertices = polygon1.vertices.Count;
            for (int i = 0; i < numvertices; i++)
            {

                int nexti = (i + 1) % numvertices;

                IVector3d bottomV1 = polygon1.vertices[i].pos;
                IVector3d topV1 = polygon2.vertices[i].pos;
                IVector3d bottomV2 = polygon1.vertices[nexti].pos;
                IVector3d topV2 = polygon2.vertices[nexti].pos;

                List<IVector3d> pPoints = new List<IVector3d> { bottomV2, topV2, topV1, bottomV1 };

                newPolygons.Add(Polygon.fromPoints(pPoints, polygon1.getStorage()));

            }

            polygon2 = polygon2.flipped();
            List<Polygon> topPolygons = PolygonUtil.concaveToConvex(polygon2);

            newPolygons.AddRange(topPolygons);

            return CSG.fromPolygons(newPolygons);

        }


        private static List<Polygon> extrude(IVector3d dir, Polygon polygon1, bool top, bool bottom)
        {
            List<Polygon> newPolygons = new List<Polygon>();


            if (bottom)
            {
                newPolygons.AddRange(PolygonUtil.concaveToConvex(polygon1));
            }

            Polygon polygon2 = polygon1.translated(dir);

            Transform rot = Transform.unity();

            IVector3d a = polygon2.getPlane().getNormal().normalized();
            IVector3d b = dir.normalized();
            
            IVector3d c = a.crossed(b);

            double l = c.magnitude(); // sine of angle

            if (l > 1e-9)
            {

                IVector3d axis = c.times(1.0 / l);
                double angle = a.angle(b);

                double sx = 0;
                double sy = 0;
                double sz = 0;

                int n = polygon2.vertices.Count;

                foreach (Vertex v in polygon2.vertices)
                {
                    sx += v.pos.x();
                    sy += v.pos.y();
                    sz += v.pos.z();
                }

                IVector3d center = Vector3d.xyz(sx / n, sy / n, sz / n);

                rot = rot.rot(center, axis, angle * Math.PI / 180.0);

                foreach (Vertex v in polygon2.vertices)
                {
                    v.pos = rot.transform(v.pos);
                }
            }

            int numvertices = polygon1.vertices.Count;
            for (int i = 0; i < numvertices; i++)
            {

                int nexti = (i + 1) % numvertices;

                IVector3d bottomV1 = polygon1.vertices[i].pos;
                IVector3d topV1 = polygon2.vertices[i].pos;
                IVector3d bottomV2 = polygon1.vertices[nexti].pos;
                IVector3d topV2 = polygon2.vertices[nexti].pos;

                List<IVector3d> pPoints = new List<IVector3d> { bottomV2, topV2, topV1, bottomV1 };

                newPolygons.Add(Polygon.fromPoints(pPoints, polygon1.getStorage()));
            }

            polygon2 = polygon2.flipped();
            List<Polygon> topPolygons = PolygonUtil.concaveToConvex(polygon2);
            if (top)
            {
                newPolygons.AddRange(topPolygons);
            }

            return newPolygons;

        }


        static List<IVector3d> toCCW(List<IVector3d> points)
        {

            List<IVector3d> result = new List<IVector3d>(points);

            if (!isCCW(Polygon.fromPoints(result)))
            {
                result.Reverse();
            }

            return result;
        }

        static List<IVector3d> toCW(List<IVector3d> points)
        {

            List<IVector3d> result = new List<IVector3d>(points);

            if (isCCW(Polygon.fromPoints(result)))
            {
                result.Reverse();
            }

            return result;
        }

        ///
        /// Indicates whether the specified polygon is defined counter-clockwise.
        /// <param name="polygon">polygon</param>
        /// <returns><c>true</c> if the specified polygon is defined counter-clockwise;</returns>
        /// <c>false</c> otherwise
        ///
        public static bool isCCW(Polygon polygon)
        {
            // thanks to Sepp Reiter for explaining me the algorithm!

            if (polygon.vertices.Count < 3)
            {
                throw new ArgumentException("Only polygons with at least 3 vertices are supported!");
            }

            // search highest left vertex
            int highestLeftVertexIndex = 0;
            Vertex highestLeftVertex = polygon.vertices[0];
            for (int i = 0; i < polygon.vertices.Count; i++)
            {
                Vertex v = polygon.vertices[i];

                if (v.pos.y() > highestLeftVertex.pos.y())
                {
                    highestLeftVertex = v;
                    highestLeftVertexIndex = i;
                }
                else if (v.pos.y() == highestLeftVertex.pos.y()
                      && v.pos.x() < highestLeftVertex.pos.x())
                {
                    highestLeftVertex = v;
                    highestLeftVertexIndex = i;
                }
            }

            // determine next and previous vertex indices
            int nextVertexIndex = (highestLeftVertexIndex + 1) % polygon.vertices.Count;
            int prevVertexIndex = highestLeftVertexIndex - 1;
            if (prevVertexIndex < 0)
            {
                prevVertexIndex = polygon.vertices.Count - 1;
            }
            Vertex nextVertex = polygon.vertices[nextVertexIndex];
            Vertex prevVertex = polygon.vertices[prevVertexIndex];

            // edge 1
            double a1 = normalizedX(highestLeftVertex.pos, nextVertex.pos);

            // edge 2
            double a2 = normalizedX(highestLeftVertex.pos, prevVertex.pos);

            // select vertex with lowest x value
            int selectedVIndex;

            if (a2 > a1)
            {
                selectedVIndex = nextVertexIndex;
            }
            else
            {
                selectedVIndex = prevVertexIndex;
            }

            if (selectedVIndex == 0
                    && highestLeftVertexIndex == polygon.vertices.Count - 1)
            {
                selectedVIndex = polygon.vertices.Count;
            }

            if (highestLeftVertexIndex == 0
                    && selectedVIndex == polygon.vertices.Count - 1)
            {
                highestLeftVertexIndex = polygon.vertices.Count;
            }

            // indicates whether edge points from highestLeftVertexIndex towards
            // the sel index (ccw)
            return selectedVIndex > highestLeftVertexIndex;
        }

        private static double normalizedX(IVector3d v1, IVector3d v2)
        {
            IVector3d v2MinusV1 = v2.minus(v1);

            return v2MinusV1.divided(v2MinusV1.magnitude()).times(Vector3d.X_ONE).x();
        }

        //    public static void main(String[] args) {
        //        System.out.println("1 CCW: " + isCCW(Polygon.fromPoints(
        //                new Vector3d(-1, -1),
        //                new Vector3d(0, -1),
        //                new Vector3d(1, 0),
        //                new Vector3d(1, 1)
        //        )));
        //
        //        System.out.println("3 CCW: " + isCCW(Polygon.fromPoints(
        //                new Vector3d(1, 1),
        //                new Vector3d(1, 0),
        //                new Vector3d(0, -1),
        //                new Vector3d(-1, -1)
        //        )));
        //
        //        System.out.println("2 CCW: " + isCCW(Polygon.fromPoints(
        //                new Vector3d(0, -1),
        //                new Vector3d(1, 0),
        //                new Vector3d(1, 1),
        //                new Vector3d(-1, -1)
        //        )));
        //
        //        System.out.println("4 CCW: " + isCCW(Polygon.fromPoints(
        //                new Vector3d(-1, -1),
        //                new Vector3d(-1, 1),
        //                new Vector3d(0, 0)
        //        )));
        //
        //        System.out.println("5 CCW: " + isCCW(Polygon.fromPoints(
        //                new Vector3d(0, 0),
        //                new Vector3d(0, 1),
        //                new Vector3d(0.5, 0.5),
        //                new Vector3d(1, 1.1),
        //                new Vector3d(1, 0)
        //        )));
        //    }
    }
}
