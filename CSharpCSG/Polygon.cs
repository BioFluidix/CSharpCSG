/*
 * Polygon.cs
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
using System.Linq;
using System.Text;
using CSharpVecMath;

namespace CSharpCSG
{
    /// <summary>
    /// Represents a convex polygon.
    /// 
    /// Each convex polygon has a <c>shared</c> property, which is shared between
    /// all polygons that are clones of each other or where split from the same
    /// polygon. This can be used to define per-polygon properties (such as surface
    /// color).
    /// </summary>
    public sealed class Polygon
    {

        /// <summary>
        /// Polygon vertices
        /// </summary>
        public readonly List<Vertex> vertices;
        /// <summary>
        /// Shared property (can be used for shared color etc.).
        /// </summary>
        private PropertyStorage shared;
        /// <summary>
        /// Plane defined by this polygon.
        /// <b>Note:</b> uses first three vertices to define the plane.
        /// </summary>
        /// 
        public readonly Plane _csg_plane;
        private CSharpVecMath.Plane plane;

        /// <summary>
        /// Returns the plane defined by this triangle. 
        /// </summary>
        /// <returns>plane</returns>
        /// 
        public CSharpVecMath.Plane getPlane()
        {
            return plane;
        }

        public void setStorage(PropertyStorage storage)
        {
            this.shared = storage;
        }

        /// <summary>
        /// Decomposes the specified concave polygon into convex polygons.
        /// </summary>
        /// <param name="points">the points that define the polygon</param>
        /// <returns>the decomposed concave polygon (list of convex polygons)</returns>
        /// 
        public static List<Polygon> fromConcavePoints(params IVector3d[] points)
        {
            Polygon p = fromPoints(points);

            return PolygonUtil.concaveToConvex(p);
        }

        /// <summary>
        /// Decomposes the specified concave polygon into convex polygons.
        /// </summary>
        /// <param name="points">the points that define the polygon</param>
        /// <returns>the decomposed concave polygon (list of convex polygons)</returns>
        /// 
        public static List<Polygon> fromConcavePoints(List<IVector3d> points)
        {
            Polygon p = fromPoints(points);

            return PolygonUtil.concaveToConvex(p);
        }

        /// <summary>
        /// Indicates whether this polyon is valid, i.e., if it
        /// </summary>
        /// <returns></returns>
        public bool isValid()
        {
            return valid;
        }

        private bool valid = true;

        /// <summary>
        /// Constructor. Creates a new polygon that consists of the specified
        /// vertices.
        /// 
        /// <b>Note:</b> the vertices used to initialize a polygon must be coplanar
        /// and form a convex loop.
        /// </summary>
        /// <param name="vertices">polygon vertices</param>
        /// <param name="shared">shared property</param>
        /// 
        public Polygon(List<Vertex> vertices, PropertyStorage shared)
        {
            this.vertices = vertices;
            this.shared = shared;
            this._csg_plane = Plane.createFromPoints(
                    vertices[0].pos,
                    vertices[1].pos,
                    vertices[2].pos);
            this.plane = CSharpVecMath.Plane.
                    fromPointAndNormal(centroid(), _csg_plane.normal);

            validateAndInit(vertices);
        }

        private void validateAndInit(List<Vertex> vertices1)
        {
            foreach (Vertex v in vertices1)
            {
                v.normal = _csg_plane.normal;
            }
            if (Vector3d.ZERO.Equals(_csg_plane.normal))
            {
                valid = false;
                Console.Error.WriteLine(
                        "Normal is zero! Probably, duplicate points have been specified!\n\n" + toStlString());
                //            throw new RuntimeException(
                //                    "Normal is zero! Probably, duplicate points have been specified!\n\n"+toStlString());
            }

            if (vertices.Count < 3)
            {
                throw new Exception(
                        "Invalid polygon: at least 3 vertices expected, got: "
                        + vertices.Count);
            }
        }

        /// <summary>
        /// Constructor. Creates a new polygon that consists of the specified
        /// vertices.
        /// 
        /// <b>Note:</b> the vertices used to initialize a polygon must be coplanar
        /// and form a convex loop.
        /// </summary>
        /// <param name="vertices">polygon vertices</param>
        /// 
        public Polygon(List<Vertex> vertices)
        {
            this.vertices = vertices;
            this._csg_plane = Plane.createFromPoints(
                    vertices[0].pos,
                    vertices[1].pos,
                    vertices[2].pos);

            this.plane = CSharpVecMath.Plane.
                    fromPointAndNormal(centroid(), _csg_plane.normal);

            validateAndInit(vertices);
        }

        /// </summary>
        /// Constructor. Creates a new polygon that consists of the specified
        /// vertices.
        /// 
        /// <b>Note:</b> the vertices used to initialize a polygon must be coplanar
        /// and form a convex loop.
        /// </summary>
        /// <param name="vertices">polygon vertices</param>
        /// 
        /// 
        public Polygon(params Vertex[] vertices) : this(vertices.ToList()) {}

        public Polygon clone()
        {
            List<Vertex> newVertices = new List<Vertex>();
            this.vertices.ForEach((vertex) => {
                newVertices.Add(vertex.clone());
            });
            return new Polygon(newVertices, getStorage());
        }

        /// <summary>
        /// Flips this polygon.
        /// </summary>
        /// <returns>this polygon</returns>
        /// 
        public Polygon flip()
        {
            vertices.ForEach((vertex) => {
                vertex.flip();
            });
            vertices.Reverse();

            _csg_plane.flip();
            this.plane = plane.flipped();

            return this;
        }

        /// <summary>
        /// Returns a flipped copy of this polygon.
        /// 
        /// <b>Note:</b> this polygon is not modified.
        /// </summary>
        /// <returns>a flipped copy of this polygon</returns>
        /// 
        public Polygon flipped()
        {
            return clone().flip();
        }

        /// <summary>
        /// Returns this polygon in STL string format.
        /// </summary>
        /// <returns>this polygon in STL string format</returns>
        /// 
        public string toStlString()
        {
            return toStlString(new StringBuilder()).ToString();
        }

        /// <summary>
        /// Returns this polygon in STL string format.
        /// </summary>
        /// 
        /// <param name="sb">string builder</param>
        /// 
        /// <returns>the specified string builder</returns>
        /// 
        public StringBuilder toStlString(StringBuilder sb)
        {

            if (this.vertices.Count >= 3)
            {

                // TODO: improve the triangulation?
                //
                // STL requires triangular polygons.
                // If our polygon has more vertices, create
                // multiple triangles:
                String firstVertexStl = this.vertices[0].toStlString();
                for (int i = 0; i < this.vertices.Count - 2; i++)
                {
                    sb.
                        Append("  facet normal ").Append(this._csg_plane.normal.toStlString()).Append("\n").
                        Append("    outer loop\n").
                        Append("      ").Append(firstVertexStl).Append("\n").
                        Append("      ");
                        this.vertices[i + 1].toStlString(sb).Append("\n").
                        Append("      ");
                        this.vertices[i + 2].toStlString(sb).Append("\n").
                        Append("    endloop\n").
                        Append("  endfacet\n");
                }
            }

            return sb;
        }

        /// <summary>
        /// Returns a triangulated version of this polygon.
        /// </summary>
        /// <returns>triangles</returns>
        /// 
        public List<Polygon> toTriangles()
        {

            List<Polygon> result = new List<Polygon>();

            if (this.vertices.Count >= 3)
            {

                // TODO: improve the triangulation?
                //
                // If our polygon has more vertices, create
                // multiple triangles:
                Vertex firstVertexStl = this.vertices[0];
                for (int i = 0; i < this.vertices.Count - 2; i++)
                {

                    // create triangle
                    Polygon polygon = Polygon.fromPoints(
                            firstVertexStl.pos,
                            this.vertices[i + 1].pos,
                            this.vertices[i + 2].pos
                    );

                    result.Add(polygon);
                }
            }

            return result;
        }

        /// <summary>
        /// Translates this polygon.
        /// </summary>
        /// <param name="v">the vector that defines the translation</param>
        /// <returns>this polygon</returns>
        /// 
        public Polygon translate(IVector3d v)
        {
            vertices.ForEach((vertex) => {
                vertex.pos = vertex.pos.plus(v);
            });

            IVector3d a = this.vertices[0].pos;
            IVector3d b = this.vertices[1].pos;
            IVector3d c = this.vertices[2].pos;

            // TODO plane update correct?
            this._csg_plane.normal = b.minus(a).crossed(c.minus(a));

            this.plane = CSharpVecMath.Plane.
                    fromPointAndNormal(centroid(), _csg_plane.normal);

            return this;
        }

        ///
        /// Returns a translated copy of this polygon.
        ///
        /// <b>Note:</b> this polygon is not modified
        ///
        /// <param name="v">the vector that defines the translation</param>
        ///
        /// <returns>a translated copy of this polygon</returns>
        ///
        public Polygon translated(IVector3d v)
        {
            return clone().translate(v);
        }

        ///
        /// Applies the specified transformation to this polygon.
        ///
        /// <b>Note:</b> if the applied transformation performs a mirror operation
        /// the vertex order of this polygon is reversed.
        ///
        /// <param name="transform">the transformation to apply</param>
        ///
        /// <returns>this polygon</returns>
        ///
        public Polygon transform(Transform transform)
        {

            this.vertices.ForEach(
                    (v)=> {
                v.transform(transform);
            }
        );

            IVector3d a = this.vertices[0].pos;
            IVector3d b = this.vertices[1].pos;
            IVector3d c = this.vertices[2].pos;

            this._csg_plane.normal = b.minus(a).crossed(c.minus(a)).normalized();
            this._csg_plane.dist = this._csg_plane.normal.dot(a);

            this.plane = CSharpVecMath.Plane.
                    fromPointAndNormal(centroid(), _csg_plane.normal);

            vertices.ForEach((vertex)=> {
                vertex.normal = plane.getNormal();
            });

            if (transform.isMirror())
            {
                // the transformation includes mirroring. flip polygon
                flip();

            }
            return this;
        }

        ///
        /// Returns a transformed copy of this polygon.
        ///
        /// <b>Note:</b> if the applied transformation performs a mirror operation
        /// the vertex order of this polygon is reversed.
        ///
        /// <b>Note:</b> this polygon is not modified
        ///
        /// <param name="transform">the transformation to apply</param>
        /// <returns>a transformed copy of this polygon</returns>
        ///
        public Polygon transformed(Transform transform)
        {
            return clone().transform(transform);
        }

        ///
        /// Creates a polygon from the specified point list.
        ///
        /// <param name="points">the points that define the polygon</param>
        /// <param name="shared">shared property storage</param>
        /// <returns>a polygon defined by the specified point list</returns>
        ///
        public static Polygon fromPoints(List<IVector3d> points,
                PropertyStorage shared)
        {
            return fromPoints(points, shared, null);
        }

        ///
        /// Creates a polygon from the specified point list.
        ///
        /// <param name="points">the points that define the polygon</param>
        /// <returns>a polygon defined by the specified point list</returns>
        ///
        public static Polygon fromPoints(List<IVector3d> points)
        {
            return fromPoints(points, new PropertyStorage(), null);
        }

        ///
        /// Creates a polygon from the specified points.
        ///
        /// <param name="points">the points that define the polygon</param>
        /// <returns>a polygon defined by the specified point list</returns>
        ///
        public static Polygon fromPoints(params IVector3d[] points)
        {
            return fromPoints(points.ToList(), new PropertyStorage(), null);
        }

        ///
        /// Creates a polygon from the specified point list.
        ///
        /// <param name="points">the points that define the polygon</param>
        /// <param name="shared">* @param plane may be null</param>
        /// <returns>a polygon defined by the specified point list</returns>
        ///
        private static Polygon fromPoints(
                List<IVector3d> points, PropertyStorage shared, Plane plane)
        {

            IVector3d normal
                    = (plane != null) ? plane.normal.clone() : null;

            if (normal == null)
            {
                normal = Plane.createFromPoints(
                        points[0],
                        points[1],
                        points[2]).normal;
            }

            List<Vertex> vertices = new List<Vertex>();

            foreach (IVector3d p in points)
            {
                IVector3d vec = p.clone();
                Vertex vertex = new Vertex(vec, normal);
                vertices.Add(vertex);
            }

            return new Polygon(vertices, shared);
        }

        ///
        /// Returns the bounds of this polygon.
        ///
        /// <returns>bouds of this polygon</returns>
        ///
        public Bounds getBounds()
        {
            double minX = Double.PositiveInfinity;
            double minY = Double.PositiveInfinity;
            double minZ = Double.PositiveInfinity;

            double maxX = Double.NegativeInfinity;
            double maxY = Double.NegativeInfinity;
            double maxZ = Double.NegativeInfinity;

            for (int i = 0; i < vertices.Count; i++)
            {

                Vertex vert = vertices[i];

                if (vert.pos.x() < minX)
                {
                    minX = vert.pos.x();
                }
                if (vert.pos.y() < minY)
                {
                    minY = vert.pos.y();
                }
                if (vert.pos.z() < minZ)
                {
                    minZ = vert.pos.z();
                }

                if (vert.pos.x() > maxX)
                {
                    maxX = vert.pos.x();
                }
                if (vert.pos.y() > maxY)
                {
                    maxY = vert.pos.y();
                }
                if (vert.pos.z() > maxZ)
                {
                    maxZ = vert.pos.z();
                }

            } // end for vertices

            return new Bounds(
                    Vector3d.xyz(minX, minY, minZ),
                    Vector3d.xyz(maxX, maxY, maxZ));
        }

        public IVector3d centroid()
        {
            IVector3d sum = Vector3d.zero();

            foreach (Vertex v in vertices)
            {
                sum = sum.plus(v.pos);
            }

            return sum.times(1.0 / vertices.Count);
        }

        ///
       /// Indicates whether the specified point is contained within this polygon.
        ///
        /// <param name="p">point</param>
        /// <returns><c>true</c> if the point is inside the polygon or on one of the</returns>
        /// edges; <c>false</c> otherwise
        ///
        public bool contains(IVector3d p)
        {

            // P not on the plane
            if (plane.distance(p) > Plane.EPSILON)
            {
                return false;
            }

            // if P is on one of the vertices, return true
            for (int n = 0; n < vertices.Count - 1; n++)
            {
                if (p.minus(vertices[n].pos).magnitude() < Plane.EPSILON)
                {
                    return true;
                }
            }

            // if P is on the plane, we proceed with projection to XY plane
            //  
            // P1--P------P2
            //     ^
            //     |
            // P is on the segment if( dist(P1,P) + dist(P2,P) - dist(P1,P2) < TOL) 
            for (int n = 0; n < vertices.Count - 1; n++)
            {

                IVector3d p1 = vertices[n].pos;
                IVector3d p2 = vertices[n + 1].pos;

                bool onASegment = p1.minus(p).magnitude() + p2.minus(p).magnitude()
                        - p1.minus(p2).magnitude() < Plane.EPSILON;

                if (onASegment)
                {
                    return true;
                }
            }

            // find projection plane
            // we start with XY plane
            int coordIndex1 = 0;
            int coordIndex2 = 1;

            bool orthogonalToXY = Math.Abs(CSharpVecMath.Plane.XY_PLANE.getNormal()
                    .dot(plane.getNormal())) < Plane.EPSILON;

            bool foundProjectionPlane = false;
            if (!orthogonalToXY && !foundProjectionPlane)
            {
                coordIndex1 = 0;
                coordIndex2 = 1;
                foundProjectionPlane = true;
            }

            bool orthogonalToXZ = Math.Abs(CSharpVecMath.Plane.XZ_PLANE.getNormal()
                    .dot(plane.getNormal())) < Plane.EPSILON;

            if (!orthogonalToXZ && !foundProjectionPlane)
            {
                coordIndex1 = 0;
                coordIndex2 = 2;
                foundProjectionPlane = true;
            }

            bool orthogonalToYZ = Math.Abs(CSharpVecMath.Plane.YZ_PLANE.getNormal()
                    .dot(plane.getNormal())) < Plane.EPSILON;

            if (!orthogonalToYZ && !foundProjectionPlane)
            {
                coordIndex1 = 1;
                coordIndex2 = 2;
                foundProjectionPlane = true;
            }

            // see from http://www.java-gaming.org/index.php?topic=26013.0
            // see http://alienryderflex.com/polygon/
            // see http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
            int i, j = vertices.Count - 1;
            bool oddNodes = false;
            double x = p.get(coordIndex1);
            double y = p.get(coordIndex2);
            for (i = 0; i < vertices.Count; i++)
            {
                double xi = vertices[i].pos.get(coordIndex1);
                double yi = vertices[i].pos.get(coordIndex2);
                double xj = vertices[j].pos.get(coordIndex1);
                double yj = vertices[j].pos.get(coordIndex2);
                if ((yi < y && yj >= y
                        || yj < y && yi >= y)
                        && (xi <= x || xj <= x))
                {
                    oddNodes ^= (xi + (y - yi) / (yj - yi) * (xj - xi) < x);
                }
                j = i;
            }
            return oddNodes;

        }

        
        public bool intersects(Polygon p)
        {
            if (!getBounds().intersects(p.getBounds()))
            {
                return false;
            }

            throw new NotSupportedException("Not implemented");
        }

        public bool contains(Polygon p)
        {

            foreach (Vertex v in p.vertices)
            {
                if (!contains(v.pos))
                {
                    return false;
                }
            }

            return true;
        }

        //    private static List<Polygon> concaveToConvex(Polygon concave) {
        //        List<Polygon> result = new ArrayList<>();
        //
        //        Triangulation t = new Triangulation();
        //        
        //        double[] xv = new double[concave.vertices.size()];
        //        double[] yv = new double[concave.vertices.size()];
        //        
        //        for(int i = 0; i < xv.length;i++) {
        //            Vector3d pos = concave.vertices.get(i).pos;
        //            xv[i] = pos.x;
        //            yv[i] = pos.y;
        //        }
        //        
        //        TriangleTri[] triangles = t.triangulatePolygon(xv, yv, xv.length);
        //        
        //        for(TriangleTri tr : triangles) {
        //            double x1 = tr.x[0];
        //            double x2 = tr.x[1];
        //            double x3 = tr.x[2];
        //            double y1 = tr.y[0];
        //            double y2 = tr.y[1];
        //            double y3 = tr.y[2];
        //            
        //            Vertex v1 = new Vertex(new Vector3d(x1, y1), new Vector3d(0, 0));
        //            Vertex v2 = new Vertex(new Vector3d(x2, y2), new Vector3d(0, 0));
        //            Vertex v3 = new Vertex(new Vector3d(x3, y3), new Vector3d(0, 0));
        //            
        //            result.add(new Polygon(v1,v2,v3));
        //        }
        //
        //        return result;
        //    }
        //    private static List<Polygon> concaveToConvex(Polygon concave) {
        //        List<Polygon> result = new ArrayList<>();
        //
        //        //convert polygon to convex polygons
        //        EarClippingTriangulator clippingTriangulator = new EarClippingTriangulator();
        //        double[] vertexArray = new double[concave.vertices.size() * 2];
        //        for (int i = 0; i < vertexArray.length; i += 2) {
        //            Vertex v = concave.vertices.get(i / 2);
        //            vertexArray[i + 0] = v.pos.x;
        //            vertexArray[i + 1] = v.pos.y;
        //        }
        // 
        //        IntArray indices = clippingTriangulator.computeTriangles(vertexArray);
        //        
        //        System.out.println("indices: " + indices.size + ", vertices: " + vertexArray.length);
        //        
        //        for (double i : vertexArray) {
        //            System.out.println("vertices: " + i);
        //        }
        //        
        //        Vertex[] newPolygonVerts = new Vertex[3];
        //
        //        int count = 0;
        //        for (int i = 0; i < indices.size; i+=2) {
        //            double x = vertexArray[indices.items[i]+0];
        //            double y = vertexArray[indices.items[i]+1];
        //            
        //            Vector3d pos = new Vector3d(x, y);
        //            Vertex v = new Vertex(pos, new Vector3d(0, 0, 0));
        //
        //            System.out.println("writing vertex: " + (count));
        //            newPolygonVerts[count] = v;
        //
        //            if (count == 2) {
        //                result.add(new Polygon(newPolygonVerts));
        //                count = 0;
        //            } else {
        //                count++;
        //            }
        //        }
        //        
        //        System.out.println("---");
        //        
        //        for (Polygon p : result) {
        //            System.out.println(p.toStlString());
        //        }
        //
        //        return result;
        //        
        ////        Point3d[] points = new Point3d[concave.vertices.size()];
        ////        
        ////        for (int i = 0; i < points.length;i++) {
        ////            Vector3d pos = concave.vertices.get(i).pos;
        ////            points[i] = new Point3d(pos.x, pos.y, pos.z);
        ////        }
        ////        
        ////        QuickHull3D hull = new QuickHull3D();
        ////        hull.build(points);
        ////
        ////        System.out.println("Vertices:");
        ////        Point3d[] vertices = hull.getVertices();
        ////        for (int i = 0; i < vertices.length; i++) {
        ////            Point3d pnt = vertices[i];
        ////            System.out.println(pnt.x + " " + pnt.y + " " + pnt.z);
        ////        }
        ////
        ////        System.out.println("Faces:");
        ////        int[][] faceIndices = hull.getFaces();
        ////        for (int i = 0; i < faceIndices.length; i++) {
        ////            for (int k = 0; k < faceIndices[i].length; k++) {
        ////                System.out.print(faceIndices[i][k] + " ");
        ////            }
        ////            System.out.println("");
        ////        }
        //
        ////        return result;
        //    }
        ///
        /// <returns>the shared</returns>
        ///
        public PropertyStorage getStorage()
        {

            if (shared == null)
            {
                shared = new PropertyStorage();
            }

            return shared;
        }
    }
}
