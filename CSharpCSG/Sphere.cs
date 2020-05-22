/*
 * Sphere.cs
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
    /// A solid sphere.
    /// 
    /// The tessellation along the longitude and latitude directions can be
    /// controlled via the <see cref="#numSlices"/> and <see cref="#numStacks"/> parameters.
    /// </summary>
    /// 
    /// author: Michael Hoffer &lt;info@michaelhoffer.de&gt;
    /// 
    public class Sphere : IPrimitive
    {

        private IVector3d center;
        private double radius;
        private int numSlices;
        private int numStacks;

        private readonly PropertyStorage properties = new PropertyStorage();

        ///
        /// Constructor. Creates a sphere with radius 1, 16 slices and 8 stacks and
        /// center [0,0,0].
        ///
        ///
        public Sphere()
        {
            init();
        }

        ///
        /// Constructor. Creates a sphere with the specified radius, 16 slices and 8
        /// stacks and center [0,0,0].
        ///
        /// <param name="radius">sphare radius</param>
        ///
        public Sphere(double radius)
        {
            init();
            this.radius = radius;
        }

        ///
        /// Constructor. Creates a sphere with the specified radius, number of slices
        /// and stacks.
        ///
        /// <param name="radius">sphare radius</param>
        /// <param name="numSlices">number of slices</param>
        /// <param name="numStacks">number of stacks</param>
        ///
        public Sphere(double radius, int numSlices, int numStacks)
        {
            init();
            this.radius = radius;
            this.numSlices = numSlices;
            this.numStacks = numStacks;
        }

        ///
        /// Constructor. Creates a sphere with the specified center, radius, number
        /// of slices and stacks.
        ///
        /// <param name="center">center of the sphere</param>
        /// <param name="radius">sphere radius</param>
        /// <param name="numSlices">number of slices</param>
        /// <param name="numStacks">number of stacks</param>
        ///
        public Sphere(IVector3d center, double radius, int numSlices, int numStacks)
        {
            this.center = center;
            this.radius = radius;
            this.numSlices = numSlices;
            this.numStacks = numStacks;
        }

        private void init()
        {
            center = Vector3d.xyz(0, 0, 0);
            radius = 1;
            numSlices = 16;
            numStacks = 8;
        }

        private Vertex sphereVertex(IVector3d c, double r, double theta, double phi)
        {
            theta *= Math.PI * 2;
            phi *= Math.PI;
            IVector3d dir = Vector3d.xyz(
                    Math.Cos(theta) * Math.Sin(phi),
                    Math.Cos(phi),
                    Math.Sin(theta) * Math.Sin(phi)
            );
            return new Vertex(c.plus(dir.times(r)), dir);
        }


        public List<Polygon> toPolygons()
        {
            List<Polygon> polygons = new List<Polygon>();

            for (int i = 0; i < numSlices; i++)
            {
                for (int j = 0; j < numStacks; j++)
                {
                    List<Vertex> vertices = new List<Vertex>();

                    vertices.Add(
                            sphereVertex(center, radius, i / (double)numSlices,
                                    j / (double)numStacks)
                    );
                    if (j > 0)
                    {
                        vertices.Add(
                                sphereVertex(center, radius, (i + 1) / (double)numSlices,
                                        j / (double)numStacks)
                        );
                    }
                    if (j < numStacks - 1)
                    {
                        vertices.Add(
                                sphereVertex(center, radius, (i + 1) / (double)numSlices,
                                        (j + 1) / (double)numStacks)
                        );
                    }
                    vertices.Add(
                            sphereVertex(center, radius, i / (double)numSlices,
                                    (j + 1) / (double)numStacks)
                    );
                    polygons.Add(new Polygon(vertices, getProperties()));
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
        /// <returns>the radius</returns>
        ///
        public double getRadius()
        {
            return radius;
        }

        ///
        /// <param name="radius">the radius to set</param>
        ///
        public void setRadius(double radius)
        {
            this.radius = radius;
        }

        ///
        /// <returns>the numSlices</returns>
        ///
        public int getNumSlices()
        {
            return numSlices;
        }

        ///
        /// <param name="numSlices">the numSlices to set</param>
        ///
        public void setNumSlices(int numSlices)
        {
            this.numSlices = numSlices;
        }

        ///
        /// <returns>the numStacks</returns>
        ///
        public int getNumStacks()
        {
            return numStacks;
        }

        ///
        /// <param name="numStacks">the numStacks to set</param>
        ///
        public void setNumStacks(int numStacks)
        {
            this.numStacks = numStacks;
        }

        public PropertyStorage getProperties()
        {
            return properties;
        }

    }
}
