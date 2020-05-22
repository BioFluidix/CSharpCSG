/*
 * Cyclinder.cs
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
    /// A solid cylinder.
    /// 
    /// The tessellation can be controlled via the <see cref="numSlices"/> parameter.
    /// </summary>
    /// 
    /// author: Michael Hoffer &lt;info@michaelhoffer.de&gt;
    /// 
    public class Cylinder : IPrimitive
    {

        private IVector3d start;
        private IVector3d end;
        private double startRadius;
        private double endRadius;
        private int numSlices;

        private readonly PropertyStorage properties = new PropertyStorage();

        /// <summary>
        /// Constructor. Creates a new cylinder with center <c>[0,0,0]</c> and
        /// ranging from <c>[0,-0.5,0]</c> to <c>[0,0.5,0]</c>, i.e.
        /// <c>size = 1</c>.
        /// </summary>
        public Cylinder()
        {
            this.start = Vector3d.xyz(0, -0.5, 0);
            this.end = Vector3d.xyz(0, 0.5, 0);
            this.startRadius = 1;
            this.endRadius = 1;
            this.numSlices = 16;
        }

        /// <summary>
        /// Constructor. Creates a cylinder ranging from <c>start</c> to <c>end</c>
        /// with the specified <c>radius</c>. The resolution of the tessellation can
        /// be controlled with <c>numSlices</c>.
        /// </summary>
        /// 
        /// <param name="start">cylinder start</param>
        /// <param name="end">cylinder end</param>
        /// <param name="radius">cylinder radius</param>
        /// <param name="numSlices">number of slices (used for tessellation)</param>
        /// 
        public Cylinder(IVector3d start, IVector3d end, double radius, int numSlices)
        {
            this.start = start;
            this.end = end;
            this.startRadius = radius;
            this.endRadius = radius;
            this.numSlices = numSlices;
        }

        /// <summary>
        /// Constructor. Creates a cylinder ranging from <c>start</c> to <c>end</c>
        /// with the specified <c>radius</c>. The resolution of the tessellation can
        /// be controlled with <c>numSlices</c>.
        /// </summary>
        /// 
        /// <param name="start">cylinder start</param> 
        /// <param name="end"> cylinder end</param>
        /// <param name="startRadius"> cylinder start radius</param>
        /// <param name="endRadius"> cylinder end radius</param>
        /// <param name="numSlices"> number of slices (used for tessellation)</param>
        /// 
        public Cylinder(IVector3d start, IVector3d end, double startRadius, double endRadius, int numSlices)
        {
            this.start = start;
            this.end = end;
            this.startRadius = startRadius;
            this.endRadius = endRadius;
            this.numSlices = numSlices;
        }

        /// <summary>
        /// Constructor. Creates a cylinder ranging from <c>[0,0,0]</c> to
        /// <c>[0,0,height]</c> with the specified <c>radius</c> and
        /// <c>height</c>. The resolution of the tessellation can be controlled with
        /// <c>numSlices</c>.
        /// </summary>
        /// 
        /// <param name="radius">cylinder radius</param>
        /// <param name="height">cylinder height</param>
        /// <param name="numSlices">number of slices (used for tessellation)</param>
        /// 
        public Cylinder(double radius, double height, int numSlices)
        {
            this.start = Vector3d.ZERO;
            this.end = Vector3d.Z_ONE.times(height);
            this.startRadius = radius;
            this.endRadius = radius;
            this.numSlices = numSlices;
        }

        /// <summary>
        /// Constructor. Creates a cylinder ranging from <c>[0,0,0]</c> to
        /// <c>[0,0,height]</c> with the specified <c>radius</c> and
        /// <c>height</c>. The resolution of the tessellation can be controlled with
        /// <c>numSlices</c>.
        /// </summary>
        /// 
        /// <param name="startRadius">cylinder start radius</param>
        /// <param name="endRadius">cylinder end radius</param>
        /// <param name="height">cylinder height</param>
        /// <param name="numSlices">number of slices (used for tessellation)</param>
        /// 
        public Cylinder(double startRadius, double endRadius, double height, int numSlices)
        {
            this.start = Vector3d.ZERO;
            this.end = Vector3d.Z_ONE.times(height);
            this.startRadius = startRadius;
            this.endRadius = endRadius;
            this.numSlices = numSlices;
        }


        public List<Polygon> toPolygons()
        {
            IVector3d s = getStart();
            IVector3d e = getEnd();
            IVector3d ray = e.minus(s);
            IVector3d axisZ = ray.normalized();
            bool isY = (Math.Abs(axisZ.y()) > 0.5);
            IVector3d axisX = Vector3d.xyz(isY ? 1 : 0, !isY ? 1 : 0, 0).
                    crossed(axisZ).normalized();
            IVector3d axisY = axisX.crossed(axisZ).normalized();
            Vertex startV = new Vertex(s, axisZ.negated());
            Vertex endV = new Vertex(e, axisZ.normalized());
            List<Polygon> polygons = new List<Polygon>();

            for (int i = 0; i < numSlices; i++)
            {
                double t0 = i / (double)numSlices, t1 = (i + 1) / (double)numSlices;
                polygons.Add(new Polygon(new List<Vertex> {
                    startV,
                    cylPoint(axisX, axisY, axisZ, ray, s, startRadius, 0, t0, -1),
                    cylPoint(axisX, axisY, axisZ, ray, s, startRadius, 0, t1, -1) },
                        properties
                ));
                polygons.Add(new Polygon(new List<Vertex> {
                    cylPoint(axisX, axisY, axisZ, ray, s, startRadius, 0, t1, 0),
                    cylPoint(axisX, axisY, axisZ, ray, s, startRadius, 0, t0, 0),
                    cylPoint(axisX, axisY, axisZ, ray, s, endRadius, 1, t0, 0),
                    cylPoint(axisX, axisY, axisZ, ray, s, endRadius, 1, t1, 0) },
                properties
                ));
                polygons.Add(new Polygon(new List<Vertex> {
                            endV,
                            cylPoint(axisX, axisY, axisZ, ray, s, endRadius, 1, t1, 1),
                            cylPoint(axisX, axisY, axisZ, ray, s, endRadius, 1, t0, 1)},
                    properties
                ));
            }

            return polygons;
        }

        private Vertex cylPoint(
                IVector3d axisX, IVector3d axisY, IVector3d axisZ, IVector3d ray, IVector3d s,
                double r, double stack, double slice, double normalBlend)
        {
            double angle = slice * Math.PI * 2;
            IVector3d ot = axisX.times(Math.Cos(angle)).plus(axisY.times(Math.Sin(angle)));
            IVector3d pos = s.plus(ray.times(stack)).plus(ot.times(r));
            IVector3d normal = ot.times(1.0 - Math.Abs(normalBlend)).plus(axisZ.times(normalBlend));
            return new Vertex(pos, normal);
        }

        /// 
        /// <returns>the start</returns>
        /// 
        public IVector3d getStart()
        {
            return start;
        }

        /// 
        /// <param name="start">the start to set</param>
        /// 
        public void setStart(IVector3d start)
        {
            this.start = start;
        }

        /// 
        /// <returns>the end</returns>
        /// 
        public IVector3d getEnd()
        {
            return end;
        }

        /// 
        /// <param name="end">the end to set</param>
        /// 
        public void setEnd(IVector3d end)
        {
            this.end = end;
        }

        /// 
        /// <returns>the radius</returns>
        /// 
        public double getStartRadius()
        {
            return startRadius;
        }

        /// 
        /// <param name="radius">the radius to set</param>
        /// 
        public void setStartRadius(double radius)
        {
            this.startRadius = radius;
        }

        /// 
        /// <returns>the radius</returns>
        /// 
        public double getEndRadius()
        {
            return endRadius;
        }

        /// 
        /// <param name="radius">the radius to set</param>
        /// 
        public void setEndRadius(double radius)
        {
            this.endRadius = radius;
        }

        /// 
        /// <returns>the number of slices</returns>
        /// 
        public int getNumSlices()
        {
            return numSlices;
        }

        /// 
        /// <param name="numSlices">the number of slices to set</param>
        /// 
        public void setNumSlices(int numSlices)
        {
            this.numSlices = numSlices;
        }

        public PropertyStorage getProperties()
        {
            return properties;
        }

    }
}
