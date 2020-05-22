/*
 * Cube.cs
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
using CSharpVecMath;

namespace CSharpCSG
{
    /// <summary>
    /// An axis-aligned solid cuboid defined by <c>center</c> and
    /// <c>dimensions</c>.
    /// </summary>
    /// 
    /// author: Michael Hoffer &lt;info@michaelhoffer.de&gt;
    /// 
    public class Cube : IPrimitive
    {

        /// <summary>
        /// Center of this cube.
        /// </summary>
        private IVector3d center;
        /// <summary>
        /// Cube dimensions.
        /// </summary>
        private IVector3d dimensions;

        private bool centered = true;

        private readonly PropertyStorage properties = new PropertyStorage();

        /// <summary>
        /// Constructor. Creates a new cube with center <c>[0,0,0]</c> and
        /// dimensions <c>[1,1,1]</c>.
        /// </summary>
        public Cube()
        {
            center = Vector3d.xyz(0, 0, 0);
            dimensions = Vector3d.xyz(1, 1, 1);
        }

        /// <summary>
        /// Constructor. Creates a new cube with center <c>[0,0,0]</c> and
        /// dimensions <c>[size,size,size]</c>.
        /// </summary>
        /// 
        /// <param name="size">size</param>
        /// 
        public Cube(double size)
        {
            center = Vector3d.xyz(0, 0, 0);
            dimensions = Vector3d.xyz(size, size, size);
        }

        /// <summary>
        /// Constructor. Creates a new cuboid with the specified center and
        /// dimensions.
        /// </summary>
        /// 
        /// <param name="center">center of the cuboid</param>
        /// <param name="dimensions">cube dimensions</param>
        /// 
        public Cube(IVector3d center, IVector3d dimensions)
        {
            this.center = center;
            this.dimensions = dimensions;
        }

        /// <summary>
        /// Constructor. Creates a new cuboid with center <c>[0,0,0]</c> and with
        /// the specified dimensions.
        /// </summary>
        /// 
        /// <param name="w">width</param>
        /// <param name="h">height</param>
        /// <param name="d">depth</param>
        /// 
        public Cube(double w, double h, double d)
            : this(Vector3d.ZERO, Vector3d.xyz(w, h, d)) { }

        //    public List<Polygon> toPolygons() {
        //        List<Polygon> result = new ArrayList<>(6);
        //
        //        Vector3d centerOffset = dimensions.times(0.5);
        //
        //        result.addAll(Arrays.asList(new Polygon[]{
        //            Polygon.fromPoints(
        //            centerOffset.times(-1, -1, -1),
        //            centerOffset.times(1, -1, -1),
        //            centerOffset.times(1, -1, 1),
        //            centerOffset.times(-1, -1, 1)
        //            ),
        //            Polygon.fromPoints(
        //            centerOffset.times(1, -1, -1),
        //            centerOffset.times(1, 1, -1),
        //            centerOffset.times(1, 1, 1),
        //            centerOffset.times(1, -1, 1)
        //            ),
        //            Polygon.fromPoints(
        //            centerOffset.times(1, 1, -1),
        //            centerOffset.times(-1, 1, -1),
        //            centerOffset.times(-1, 1, 1),
        //            centerOffset.times(1, 1, 1)
        //            ),
        //            Polygon.fromPoints(
        //            centerOffset.times(1, 1, 1),
        //            centerOffset.times(-1, 1, 1),
        //            centerOffset.times(-1, -1, 1),
        //            centerOffset.times(1, -1, 1)
        //            ),
        //            Polygon.fromPoints(
        //            centerOffset.times(-1, 1, 1),
        //            centerOffset.times(-1, 1, -1),
        //            centerOffset.times(-1, -1, -1),
        //            centerOffset.times(-1, -1, 1)
        //            ),
        //            Polygon.fromPoints(
        //            centerOffset.times(-1, 1, -1),
        //            centerOffset.times(1, 1, -1),
        //            centerOffset.times(1, -1, -1),
        //            centerOffset.times(-1, -1, -1)
        //            )
        //        }
        //        ));
        //        
        //        if(!centered) {
        //            Transform centerTransform = Transform.unity().
        //                    translate(dimensions.x() / 2.0,
        //                            dimensions.y() / 2.0,
        //                            dimensions.z() / 2.0);
        //
        //            for (Polygon p : result) {
        //                p.transform(centerTransform);
        //            }
        //        }
        //
        //        return result;
        //    }


        public List<Polygon> toPolygons()
        {

            int[][][] a = new int[][][] {
            // position     // normal
            new int [][] { new int[] {0, 4, 6, 2}, new int[] {-1, 0, 0}},
            new int [][] { new int[] {1, 3, 7, 5}, new int[] { +1, 0, 0}},
            new int [][] { new int[] {0, 1, 5, 4}, new int[] { 0, -1, 0}},
            new int [][] { new int[] {2, 6, 7, 3}, new int[] { 0, +1, 0}},
            new int [][] { new int[] {0, 2, 3, 1}, new int[] { 0, 0, -1}},
            new int [][] { new int[] {4, 5, 7, 6}, new int[] { 0, 0, +1}}
        };
            List<Polygon> polygons = new List<Polygon>();
            foreach (int[][] info in a)
            {
                List<Vertex> vertices = new List<Vertex>();
                foreach (int i in info[0])
                {
                    IVector3d pos = Vector3d.xyz(
                            center.x() + dimensions.x() * (1 * Math.Min(1, i & 1) - 0.5),
                            center.y() + dimensions.y() * (1 * Math.Min(1, i & 2) - 0.5),
                            center.z() + dimensions.z() * (1 * Math.Min(1, i & 4) - 0.5)
                    );
                    vertices.Add(new Vertex(pos, Vector3d.xyz(
                            (double)info[1][0],
                            (double)info[1][1],
                            (double)info[1][2]
                    )));
                }
                polygons.Add(new Polygon(vertices, properties));
            }

            if (!centered)
            {

                Transform centerTransform = Transform.unity().
                        translate(dimensions.x() / 2.0,
                                dimensions.y() / 2.0,
                                dimensions.z() / 2.0);

                foreach (Polygon p in polygons)
                {
                    p.transform(centerTransform);
                }
            }

            return polygons;
        }

        /// 
        /// <returns>the center</returns>
        /// 
        public IVector3d getCenter()
        {
            return center;
        }

        /// 
        /// <param name="center">the center to set</param>
        /// 
        public void setCenter(IVector3d center)
        {
            this.center = center;
        }

        /// 
        /// <returns>the dimensions</returns>
        /// 
        public IVector3d getDimensions()
        {
            return dimensions;
        }

        /// 
        /// <param name="dimensions">the dimensions to set</param>
        /// 
        public void setDimensions(IVector3d dimensions)
        {
            this.dimensions = dimensions;
        }


        public PropertyStorage getProperties()
        {
            return properties;
        }

        /// <summary>
        /// Defines that this cube will not be centered.
        /// </summary>
        /// 
        /// <returns>this cube</returns>
        /// 
        public Cube noCenter()
        {
            centered = false;
            return this;
        }

    }
}
