/*
 * Plane.cs
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


using System.Collections.Generic;
using CSharpVecMath;

namespace CSharpCSG
{
    /// <summary>
    /// Represents a plane in 3D space.
    /// </summary>
    /// author: Michael Hoffer &lt;info@michaelhoffer.de&gt;
    /// 
    public class Plane
    {

        /// 
        /// EPSILON is the tolerance used by {@link #splitPolygon(Polygon, java.util.List, java.util.List, java.util.List, java.util.List)
        /// } to decide if a point is on the plane.
        /// 
        public const double EPSILON = 1e-8;

        /// <summary>
        /// XY plane.
        /// </summary>
        public static readonly Plane XY_PLANE = new Plane(Vector3d.Z_ONE, 1);
        /// <summary>
        /// XZ plane.
        /// </summary>
        public static readonly Plane XZ_PLANE = new Plane(Vector3d.Y_ONE, 1);
        /// <summary>
        /// YZ plane.
        /// </summary>
        public static readonly Plane YZ_PLANE = new Plane(Vector3d.X_ONE, 1);

        /// <summary>
        /// Normal vector.
        /// </summary>
        public IVector3d normal;
        /// <summary>
        /// Distance to origin.
        /// </summary>
        public double dist;

        /// <summary>
        /// Constructor. Creates a new plane defined by its normal vector and the
        /// distance to the origin.
        /// </summary>
        /// <param name="normal">plane normal</param>
        /// <param name="dist">distance from origin</param>
        /// 
        public Plane(IVector3d normal, double dist)
        {
            this.normal = normal.normalized();
            this.dist = dist;
        }

        /// <summary>
        /// Creates a plane defined by the the specified points.
        /// </summary>
        /// <param name="a">first point</param>
        /// <param name="b">second point</param>
        /// <param name="c">third point</param>
        /// <returns>a plane</returns>
        /// 
        public static Plane createFromPoints(IVector3d a, IVector3d b, IVector3d c)
        {
            IVector3d n = b.minus(a).crossed(c.minus(a)).normalized();
            return new Plane(n, n.dot(a));
        }

        public Plane clone()
        {
            return new Plane(normal.clone(), dist);
        }

        /// <summary>
        /// Flips this plane.
        /// </summary>
        public void flip()
        {
            normal = normal.negated();
            dist = -dist;
        }

        /// <summary>
        /// Splits a <see cref="Polygon"/> by this plane if needed. After that it puts the
        /// polygons or the polygon fragments in the appropriate lists
        /// (<c>front</c>, <c>back</c>). Coplanar polygons go into either
        /// <c>coplanarFront</c>, <c>coplanarBack</c> depending on their
        /// orientation with respect to this plane. Polygons in front or back of this
        /// plane go into either <c>front</c> or <c>back</c>.
        /// </summary>
        /// <param name="polygon">polygon to split</param>
        /// <param name="coplanarFront">"coplanar front" polygons</param>
        /// <param name="coplanarBack">"coplanar back" polygons</param>
        /// <param name="front">front polygons</param>
        /// <param name="back">back polgons</param>
        /// 
        public void splitPolygon(
                Polygon polygon,
                List<Polygon> coplanarFront,
                List<Polygon> coplanarBack,
                List<Polygon> front,
                List<Polygon> back)
        {
            const int COPLANAR = 0;
            const int FRONT = 1;
            const int BACK = 2;
            const int SPANNING = 3; // == some in the FRONT + some in the BACK

            // Classify each point as well as the entire polygon into one of the 
            // above four classes.
            int polygonType = 0;
            List<int> types = new List<int>(polygon.vertices.Count);
            for (int i = 0; i < polygon.vertices.Count; i++)
            {
                double t = this.normal.dot(polygon.vertices[i].pos) - this.dist;
                int type = (t < -Plane.EPSILON) ? BACK : (t > Plane.EPSILON) ? FRONT : COPLANAR;
                polygonType |= type;
                types.Add(type);
            }

            //System.out.println("> switching");
            // Put the polygon in the correct list, splitting it when necessary.
            switch (polygonType)
            {
                case COPLANAR:
                    //System.out.println(" -> coplanar");
                    (this.normal.dot(polygon._csg_plane.normal) > 0 ? coplanarFront : coplanarBack).Add(polygon);
                    break;
                case FRONT:
                    //System.out.println(" -> front");
                    front.Add(polygon);
                    break;
                case BACK:
                    //System.out.println(" -> back");
                    back.Add(polygon);
                    break;
                case SPANNING:
                    //System.out.println(" -> spanning");
                    List<Vertex> f = new List<Vertex>();
                    List<Vertex> b = new List<Vertex>();
                    for (int i = 0; i < polygon.vertices.Count; i++)
                    {
                        int j = (i + 1) % polygon.vertices.Count;
                        int ti = types[i];
                        int tj = types[j];
                        Vertex vi = polygon.vertices[i];
                        Vertex vj = polygon.vertices[j];
                        if (ti != BACK)
                        {
                            f.Add(vi);
                        }
                        if (ti != FRONT)
                        {
                            b.Add(ti != BACK ? vi.clone() : vi);
                        }
                        if ((ti | tj) == SPANNING)
                        {
                            double t = (this.dist - this.normal.dot(vi.pos))
                                    / this.normal.dot(vj.pos.minus(vi.pos));
                            Vertex v = vi.interpolate(vj, t);
                            f.Add(v);
                            b.Add(v.clone());
                        }
                    }
                    if (f.Count >= 3)
                    {
                        front.Add(new Polygon(f, polygon.getStorage()));
                    }
                    if (b.Count >= 3)
                    {
                        back.Add(new Polygon(b, polygon.getStorage()));
                    }
                    break;
            }
        }
    }
}
