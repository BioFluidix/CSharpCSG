/*
 * CSG.cs
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
using System.Windows.Media;
using System.Windows.Media.Media3D;
using CSharpVecMath;

namespace CSharpCSG
{

    internal class PolygonStruct
    {

        public PropertyStorage storage;
        public List<int> indices;
        public string materialName;

        public PolygonStruct(PropertyStorage storage, List<int> indices, string materialName)
        {
            this.storage = storage;
            this.indices = indices;
            this.materialName = materialName;
        }
    }


    public class CSG
    {

        public enum OptType
        {
            CSG_BOUND,
            POLYGON_BOUND,
            NONE
        }



        private List<Polygon> polygons;
        private static OptType defaultOptType = OptType.NONE;
        private OptType? optType = null;
        private PropertyStorage storage;



        /// 
        /// <returns>the optType</returns>
        /// 
        private OptType getOptType()
        {
            return optType ?? defaultOptType;
        }

        /// 
        /// <param name="optType">the optType to set</param>
        /// 
        public static void setDefaultOptType(OptType optType)
        {
            defaultOptType = optType;
        }

        /// 
        /// <param name="optType">the optType to set</param>
        /// 
        public void setOptType(OptType? optType)
        {
            this.optType = optType;
        }

        /// <summary>
        /// Returns the bounds of this csg.
        /// </summary>
        /// 
        /// <returns>bouds of this csg</returns>
        /// 
        public Bounds getBounds()
        {

            if (polygons.Count == 0)
            {
                return new Bounds(Vector3d.ZERO, Vector3d.ZERO);
            }

            IVector3d initial = polygons[0].vertices[0].pos;

            double minX = initial.x();
            double minY = initial.y();
            double minZ = initial.z();

            double maxX = initial.x();
            double maxY = initial.y();
            double maxZ = initial.z();

            foreach (Polygon p in getPolygons())
            {

                for (int i = 0; i < p.vertices.Count; i++)
                {

                    Vertex vert = p.vertices[i];

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

            } // end for polygon

            return new Bounds(
                    Vector3d.xyz(minX, minY, minZ),
                    Vector3d.xyz(maxX, maxY, maxZ));
        }




        private CSG()
        {
            storage = new PropertyStorage();
        }

        /// <summary>
        /// Constructs a CSG from a list of <see cref="Polygon"/> instances.
        /// </summary>
        /// 
        /// <param name="polygons">polygons</param>
        /// <returns>a CSG instance</returns>
        /// 
        public static CSG fromPolygons(List<Polygon> polygons)
        {

            CSG csg = new CSG();
            csg.polygons = polygons;

            return csg;
        }

        /// <summary>
        /// Constructs a CSG from the specified <see cref="Polygon"/> instances.
        /// </summary>
        /// 
        /// <param name="polygons">polygons</param>
        /// <returns>a CSG instance</returns>
        /// 
        public static CSG fromPolygons(params Polygon[] polygons)
        {
            return fromPolygons(polygons.ToList());
        }

        /// <summary>
        /// Constructs a CSG from a list of <see cref="Polygon"/> instances.
        /// </summary>
        /// 
        /// <param name="storage">shared storage</param>
        /// <param name="polygons">polygons</param>
        /// <returns>a CSG instance</returns>
        /// 
        public static CSG fromPolygons(PropertyStorage storage, List<Polygon> polygons)
        {

            CSG csg = new CSG();
            csg.polygons = polygons;

            csg.storage = storage;

            foreach (Polygon polygon in polygons)
            {
                polygon.setStorage(storage);
            }

            return csg;
        }

        /// <summary>
        /// Constructs a CSG from the specified <see cref="Polygon"/> instances.
        /// </summary>
        /// 
        /// <param name="storage">shared storage</param>
        /// <param name="polygons">polygons</param>
        /// <returns>a CSG instance</returns>
        /// 
        public static CSG fromPolygons(PropertyStorage storage, params Polygon[] polygons)
        {
            return fromPolygons(storage, polygons.ToList());
        }


        public CSG clone()
        {
            CSG csg = new CSG();

            csg.setOptType(this.getOptType());

            // sequential code
            //        csg.polygons = new ArrayList<>();
            //        polygons.forEach((polygon) -> {
            //            csg.polygons.add(polygon.clone());
            //        });

            var selector = new Func<Polygon, Polygon>(p => p.clone());
            if (polygons.Count > 200)
            {
                csg.polygons = polygons.AsParallel().Select(selector).ToList();
            }
            else
            {
                csg.polygons = polygons.Select(selector).ToList();
            }

            return csg;
        }

        /// 
        /// 
        /// <returns>the polygons of this CSG</returns>
        /// 
        public List<Polygon> getPolygons()
        {
            return polygons;
        }

        /// <summary>
        /// Defines the CSg optimization type.
        /// </summary>
        /// 
        /// <param name="type">optimization type</param>
        /// <returns>this CSG</returns>
        /// 
        public CSG optimization(OptType? type)
        {
            this.setOptType(type);
            return this;
        }

        /// <summary>
        /// Return a new CSG solid representing the union of this csg and the specified csg.
        /// 
        /// <b>Note:</b> Neither this csg nor the specified csg are weighted.
        /// 
        /// <blockquote><pre>
        ///    A.union(B)
        /// 
        ///    +-------+            +-------+
        ///    |       |            |       |
        ///    |   A   |            |       |
        ///    |    +--+----+   =   |       +----+
        ///    +----+--+    |       +----+       |
        ///         |   B   |            |       |
        ///         |       |            |       |
        ///         +-------+            +-------+
        /// </pre></blockquote>
        /// </summary>
        /// 
        /// <param name="csg">other csg</param>
        /// 
        /// <returns>union of this csg and the specified csg</returns>
        /// 
        public CSG union(CSG csg)
        {
            switch (getOptType())
            {
                case OptType.CSG_BOUND:
                    return _unionCSGBoundsOpt(csg);
                case OptType.POLYGON_BOUND:
                    return _unionPolygonBoundsOpt(csg);
                default:
                    //                return _unionIntersectOpt(csg);
                    return _unionNoOpt(csg);
            }
        }

        /// <summary>
        /// Returns a csg consisting of the polygons of this csg and the specified csg.
        /// 
        /// The purpose of this method is to allow fast union operations for objects that do not intersect.
        /// 
        /// </summary>
        /// 
        /// <p>
        /// <b>WARNING:</b> this method does not apply the csg algorithms. Therefore, please ensure that this csg and the
        /// specified csg do not intersect.
        /// 
        /// <param name="csg">csg</param>
        /// 
        /// <returns>a csg consisting of the polygons of this csg and the specified csg</returns>
        /// 
        public CSG dumbUnion(CSG csg)
        {

            CSG result = this.clone();
            CSG other = csg.clone();

            result.polygons.AddRange(other.polygons);

            return result;
        }

        /// <summary>
        /// Return a new CSG solid representing the union of this csg and the specified csgs.
        /// </summary>
        /// 
        /// <b>Note:</b> Neither this csg nor the specified csg are weighted.
        /// 
        /// <blockquote><pre>
        ///    A.union(B)
        /// 
        ///    +-------+            +-------+
        ///    |       |            |       |
        ///    |   A   |            |       |
        ///    |    +--+----+   =   |       +----+
        ///    +----+--+    |       +----+       |
        ///         |   B   |            |       |
        ///         |       |            |       |
        ///         +-------+            +-------+
        /// </pre></blockquote>
        /// 
        /// 
        /// <param name="csgs">other csgs</param>
        /// 
        /// <returns>union of this csg and the specified csgs</returns>
        /// 
        public CSG union(List<CSG> csgs)
        {

            CSG result = this;

            foreach (CSG csg in csgs)
            {
                result = result.union(csg);
            }

            return result;
        }

        /// <summary>
        /// Return a new CSG solid representing the union of this csg and the specified csgs.
        /// </summary>
        /// 
        /// <b>Note:</b> Neither this csg nor the specified csg are weighted.
        /// 
        /// <blockquote><pre>
        ///    A.union(B)
        /// 
        ///    +-------+            +-------+
        ///    |       |            |       |
        ///    |   A   |            |       |
        ///    |    +--+----+   =   |       +----+
        ///    +----+--+    |       +----+       |
        ///         |   B   |            |       |
        ///         |       |            |       |
        ///         +-------+            +-------+
        /// </pre></blockquote>
        /// 
        /// 
        /// <param name="csgs">other csgs</param>
        /// 
        /// <returns>union of this csg and the specified csgs</returns>
        /// 
        public CSG union(params CSG[] csgs)
        {
            return union(csgs.ToList());
        }

        /// <summary>
        /// Returns the convex hull of this csg.
        /// </summary>
        /// 
        /// <returns>the convex hull of this csg</returns>
        /// 
        public CSG hull()
        {
            return HullUtil.hull(this, storage);
        }

        /// <summary>
        /// Returns the convex hull of this csg and the union of the specified csgs.
        /// </summary>
        /// 
        /// <param name="csgs">csgs</param>
        /// <returns>the convex hull of this csg and the specified csgs</returns>
        /// 
        public CSG hull(List<CSG> csgs)
        {

            CSG csgsUnion = new CSG();
            csgsUnion.storage = storage;
            csgsUnion.optType = optType;
            csgsUnion.polygons = this.clone().polygons;

            csgs.ForEach(csg =>
            {
                csgsUnion.polygons.AddRange(csg.clone().polygons);
            });

            csgsUnion.polygons.ForEach(p => p.setStorage(storage));
            return csgsUnion.hull();

            //        CSG csgsUnion = this;
            //
            //        for (CSG csg : csgs) {
            //            csgsUnion = csgsUnion.union(csg);
            //        }
            //
            //        return csgsUnion.hull();
        }

        /// <summary>
        /// Returns the convex hull of this csg and the union of the specified csgs.
        /// </summary>
        /// 
        /// <param name="csgs">csgs</param>
        /// <returns>the convex hull of this csg and the specified csgs</returns>
        /// 
        public CSG hull(params CSG[] csgs)
        {

            return hull(csgs.ToList());
        }

        private CSG _unionCSGBoundsOpt(CSG csg)
        {
            Console.Error.WriteLine("WARNING: using " + OptType.NONE
                    + " since other optimization types missing for union operation.");
            return _unionIntersectOpt(csg);
        }

        private CSG _unionPolygonBoundsOpt(CSG csg)
        {
            List<Polygon> inner = new List<Polygon>();
            List<Polygon> outer = new List<Polygon>();

            Bounds bounds = csg.getBounds();

            this.polygons.ForEach(p =>
            {
                if (bounds.intersects(p.getBounds()))
                {
                    inner.Add(p);
                }
                else
                {
                    outer.Add(p);
                }
            });

            List<Polygon> allPolygons = new List<Polygon>();

            if (inner.Count != 0)
            {
                CSG innerCSG = CSG.fromPolygons(inner);

                allPolygons.AddRange(outer);
                allPolygons.AddRange(innerCSG._unionNoOpt(csg).polygons);
            }
            else
            {
                allPolygons.AddRange(this.polygons);
                allPolygons.AddRange(csg.polygons);
            }

            return CSG.fromPolygons(allPolygons).optimization(getOptType());
        }

        /// <summary>
        /// Optimizes for intersection. If csgs do not intersect create a new csg that consists of the polygon lists of this
        /// csg and the specified csg. In this case no further space partitioning is performed.
        /// </summary>
        /// 
        /// <param name="csg">csg</param>
        /// <returns>the union of this csg and the specified csg</returns>
        /// 
        private CSG _unionIntersectOpt(CSG csg)
        {
            bool intersects = false;

            Bounds bounds = csg.getBounds();

            foreach (Polygon p in polygons)
            {
                if (bounds.intersects(p.getBounds()))
                {
                    intersects = true;
                    break;
                }
            }

            List<Polygon> allPolygons = new List<Polygon>();

            if (intersects)
            {
                return _unionNoOpt(csg);
            }
            else
            {
                allPolygons.AddRange(this.polygons);
                allPolygons.AddRange(csg.polygons);
            }

            return CSG.fromPolygons(allPolygons).optimization(getOptType());
        }

        private CSG _unionNoOpt(CSG csg)
        {
            Node a = new Node(this.clone().polygons);
            Node b = new Node(csg.clone().polygons);
            a.clipTo(b);
            b.clipTo(a);
            b.invert();
            b.clipTo(a);
            b.invert();
            a.build(b.allPolygons());
            return CSG.fromPolygons(a.allPolygons()).optimization(getOptType());
        }

        /// <summary>
        /// Return a new CSG solid representing the difference of this csg and the specified csgs.
        /// </summary>
        /// 
        /// <b>Note:</b> Neither this csg nor the specified csgs are weighted.
        /// 
        /// <code>
        /// A.difference(B)
        /// 
        /// +-------+            +-------+
        /// |       |            |       |
        /// |   A   |            |       |
        /// |    +--+----+   =   |    +--+
        /// +----+--+    |       +----+
        ///      |   B   |
        ///      |       |
        ///      +-------+
        /// </code>
        /// 
        /// <param name="csgs">other csgs</param>
        /// <returns>difference of this csg and the specified csgs</returns>
        /// 
        public CSG difference(List<CSG> csgs)
        {

            if (csgs.Count == 0)
            {
                return this.clone();
            }

            CSG csgsUnion = csgs[0];

            for (int i = 1; i < csgs.Count; i++)
            {
                csgsUnion = csgsUnion.union(csgs[i]);
            }

            return difference(csgsUnion);
        }

        /// <summary>
        /// Return a new CSG solid representing the difference of this csg and the specified csgs.
        /// </summary>
        /// 
        /// <b>Note:</b> Neither this csg nor the specified csgs are weighted.
        /// 
        /// <code>
        /// A.difference(B)
        /// 
        /// +-------+            +-------+
        /// |       |            |       |
        /// |   A   |            |       |
        /// |    +--+----+   =   |    +--+
        /// +----+--+    |       +----+
        ///      |   B   |
        ///      |       |
        ///      +-------+
        /// </code>
        /// 
        /// <param name="csgs">other csgs</param>
        /// <returns>difference of this csg and the specified csgs</returns>
        /// 
        public CSG difference(params CSG[] csgs)
        {

            return difference(csgs.ToList());
        }

        /// <summary>
        /// Return a new CSG solid representing the difference of this csg and the specified csg.
        /// </summary>
        /// 
        /// <b>Note:</b> Neither this csg nor the specified csg are weighted.
        /// 
        /// <code>
        /// A.difference(B)
        /// 
        /// +-------+            +-------+
        /// |       |            |       |
        /// |   A   |            |       |
        /// |    +--+----+   =   |    +--+
        /// +----+--+    |       +----+
        ///      |   B   |
        ///      |       |
        ///      +-------+
        /// </code>
        /// 
        /// <param name="csg">other csg</param>
        /// <returns>difference of this csg and the specified csg</returns>
        /// 
        public CSG difference(CSG csg)
        {
            switch (getOptType())
            {
                case OptType.CSG_BOUND:
                    return _differenceCSGBoundsOpt(csg);
                case OptType.POLYGON_BOUND:
                    return _differencePolygonBoundsOpt(csg);
                default:
                    return _differenceNoOpt(csg);
            }
        }

        private CSG _differenceCSGBoundsOpt(CSG csg)
        {
            CSG b = csg;

            CSG a1 = this._differenceNoOpt(csg.getBounds().toCSG());
            CSG a2 = this.intersect(csg.getBounds().toCSG());

            return a2._differenceNoOpt(b)._unionIntersectOpt(a1).optimization(getOptType());
        }

        private CSG _differencePolygonBoundsOpt(CSG csg)
        {
            List<Polygon> inner = new List<Polygon>();
            List<Polygon> outer = new List<Polygon>();

            Bounds bounds = csg.getBounds();

            this.polygons.ForEach(p =>
            {
                if (bounds.intersects(p.getBounds()))
                {
                    inner.Add(p);
                }
                else
                {
                    outer.Add(p);
                }
            });

            CSG innerCSG = CSG.fromPolygons(inner);

            List<Polygon> allPolygons = new List<Polygon>();
            allPolygons.AddRange(outer);
            allPolygons.AddRange(innerCSG._differenceNoOpt(csg).polygons);

            return CSG.fromPolygons(allPolygons).optimization(getOptType());
        }

        private CSG _differenceNoOpt(CSG csg)
        {

            Node a = new Node(this.clone().polygons);
            Node b = new Node(csg.clone().polygons);

            a.invert();
            a.clipTo(b);
            b.clipTo(a);
            b.invert();
            b.clipTo(a);
            b.invert();
            a.build(b.allPolygons());
            a.invert();

            CSG csgA = CSG.fromPolygons(a.allPolygons()).optimization(getOptType());
            return csgA;
        }

        /// <summary>
        /// Return a new CSG solid representing the intersection of this csg and the specified csg.
        /// </summary>
        /// 
        /// <b>Note:</b> Neither this csg nor the specified csg are weighted.
        /// 
        /// <code>
        ///     A.intersect(B)
        /// 
        ///     +-------+
        ///     |       |
        ///     |   A   |
        ///     |    +--+----+   =   +--+
        ///     +----+--+    |       +--+
        ///          |   B   |
        ///          |       |
        ///          +-------+
        /// }
        /// </code>
        /// 
        /// <param name="csg">other csg</param>
        /// <returns>intersection of this csg and the specified csg</returns>
        /// 
        public CSG intersect(CSG csg)
        {

            Node a = new Node(this.clone().polygons);
            Node b = new Node(csg.clone().polygons);
            a.invert();
            b.clipTo(a);
            b.invert();
            a.clipTo(b);
            b.clipTo(a);
            a.build(b.allPolygons());
            a.invert();
            return CSG.fromPolygons(a.allPolygons()).optimization(getOptType());
        }

        /// <summary>
        /// Return a new CSG solid representing the intersection of this csg and the specified csgs.
        /// </summary>
        /// 
        /// <b>Note:</b> Neither this csg nor the specified csgs are weighted.
        /// 
        /// <code>
        ///     A.intersect(B)
        /// 
        ///     +-------+
        ///     |       |
        ///     |   A   |
        ///     |    +--+----+   =   +--+
        ///     +----+--+    |       +--+
        ///          |   B   |
        ///          |       |
        ///          +-------+
        /// }
        /// </code>
        /// 
        /// <param name="csgs">other csgs</param>
        /// <returns>intersection of this csg and the specified csgs</returns>
        /// 
        public CSG intersect(List<CSG> csgs)
        {

            if (csgs.Count == 0)
            {
                return this.clone();
            }

            CSG csgsUnion = csgs[0];

            for (int i = 1; i < csgs.Count; i++)
            {
                csgsUnion = csgsUnion.union(csgs[i]);
            }

            return intersect(csgsUnion);
        }

        /// <summary>
        /// Return a new CSG solid representing the intersection of this csg and the specified csgs.
        /// </summary>
        /// 
        /// <b>Note:</b> Neither this csg nor the specified csgs are weighted.
        /// 
        /// <code>
        ///     A.intersect(B)
        /// 
        ///     +-------+
        ///     |       |
        ///     |   A   |
        ///     |    +--+----+   =   +--+
        ///     +----+--+    |       +--+
        ///          |   B   |
        ///          |       |
        ///          +-------+
        /// }
        /// </code>
        /// 
        /// <param name="csgs">other csgs</param>
        /// <returns>intersection of this csg and the specified csgs</returns>
        /// 
        public CSG intersect(params CSG[] csgs)
        {

            return intersect(csgs.ToList());
        }

        /// <summary>
        /// Returns this csg in STL string format.
        /// </summary>
        /// 
        /// <returns>this csg in STL string format</returns>
        /// 
        public string toStlString()
        {
            StringBuilder sb = new StringBuilder();
            toStlString(sb);
            return sb.ToString();
        }


        /// <summary>
        /// Returns this csg in STL string format.
        /// </summary>
        /// 
        /// <param name="sb">string builder</param>
        /// 
        /// <returns>the specified string builder</returns>
        /// 
        public StringBuilder toStlString(StringBuilder sb)
        {
            sb.Append("solid v3d.csg\n");
            this.polygons.ForEach(
                    (Polygon p) =>
                    {
                        p.toStlString(sb);
                    });
            sb.Append("endsolid v3d.csg\n");
            return sb;
        }

        public CSG color(Color c)
        {
            CSG result = this.clone();
            storage.set("material:color", $"{c.R} {c.G} {c.B}");
            return result;
        }

        public ObjFile toObj()
        {
            // we triangulate the polygon to ensure 
            // compatibility with 3d printer software
            return toObj(3);
        }

        public ObjFile toObj(int maxNumberOfVerts)
        {
            if (maxNumberOfVerts != 3)
            {
                throw new NotSupportedException("maxNumberOfVerts > 3 not supported yet");
            }

            StringBuilder objSb = new StringBuilder();

            objSb.Append("mtllib " + ObjFile.MTL_NAME);

            objSb.Append("# Group").Append("\n");
            objSb.Append("g v3d.csg\n");


            List<Vertex> vertices = new List<Vertex>();
            List<PolygonStruct> indices = new List<PolygonStruct>();

            objSb.Append("\n# Vertices\n");

            Dictionary<PropertyStorage, int> materialNames = new Dictionary<PropertyStorage, int>();

            int materialIndex = 0;

            foreach (Polygon p in polygons)
            {
                List<int> polyIndices = new List<int>();

                p.vertices.ForEach(v =>
                {
                    if (!vertices.Contains(v))
                    {
                        vertices.Add(v);
                        v.toObjString(objSb);
                        polyIndices.Add(vertices.Count);
                    }
                    else
                    {
                        polyIndices.Add(vertices.IndexOf(v) + 1);
                    }
                });

                if (!materialNames.ContainsKey(p.getStorage()))
                {
                    materialIndex++;
                    materialNames[p.getStorage()] = materialIndex;
                    p.getStorage().set("material:name", materialIndex);
                }

                indices.Add(new PolygonStruct(
                    p.getStorage(), polyIndices,
                    "material-" + materialNames[p.getStorage()]));
            }

            objSb.Append("\n# Faces").Append("\n");

            foreach (PolygonStruct ps in indices)
            {

                // add mtl info
                if (ps.storage.contains("material:color"))
                {
                    objSb.Append("usemtl ").Append(ps.materialName).Append("\n");
                }

                // we triangulate the polygon to ensure 
                // compatibility with 3d printer software
                List<int> pVerts = ps.indices;
                int index1 = pVerts[0];
                for (int i = 0; i < pVerts.Count - 2; i++)
                {
                    int index2 = pVerts[i + 1];
                    int index3 = pVerts[i + 2];

                    objSb.Append("f ").
                        Append(index1).Append(" ").
                        Append(index2).Append(" ").
                        Append(index3).Append("\n");
                }
                //
                //            objSb.Append("f ");
                //            for (int i = 0; i < pVerts.size(); i++) {
                //                objSb.Append(pVerts.get(i)).Append(" ");
                //            }
                objSb.Append("\n");
            }

            objSb.Append("\n# End Group v3d.csg").Append("\n");

            StringBuilder mtlSb = new StringBuilder();

            materialNames.Keys.ToList().ForEach(s =>
            {
                if (s.contains("material:color"))
                {
                    var matColor = s.getValue<Color>("material:color");
                    var matName = s.getValue<string>("material:name");
                    mtlSb.Append("newmtl material-").Append(matName).Append("\n");
                    mtlSb.Append("Kd ").Append(matColor).Append("\n");
                }
            });

            return new ObjFile(objSb.ToString(), mtlSb.ToString());
        }

        /// <summary>
        /// Returns this csg in OBJ string format.
        /// </summary>
        /// 
        /// <param name="sb">string builder</param>
        /// <returns>the specified string builder</returns>
        /// 
        public StringBuilder toObjString(StringBuilder sb)
        {
            sb.Append("# Group").Append("\n");
            sb.Append("g v3d.csg\n");


            List<Vertex> vertices = new List<Vertex>();
            List<PolygonStruct> indices = new List<PolygonStruct>();

            sb.Append("\n# Vertices\n");

            foreach (Polygon p in polygons)
            {
                List<int> polyIndices = new List<int>();

                p.vertices.ForEach(v =>
                {
                    if (!vertices.Contains(v))
                    {
                        vertices.Add(v);
                        v.toObjString(sb);
                        polyIndices.Add(vertices.Count);
                    }
                    else
                    {
                        polyIndices.Add(vertices.IndexOf(v) + 1);
                    }
                });

            }

            sb.Append("\n# Faces").Append("\n");

            foreach (PolygonStruct ps in indices)
            {
                // we triangulate the polygon to ensure 
                // compatibility with 3d printer software
                List<int> pVerts = ps.indices;
                int index1 = pVerts[0];
                for (int i = 0; i < pVerts.Count - 2; i++)
                {
                    int index2 = pVerts[i + 1];
                    int index3 = pVerts[i + 2];

                    sb.Append("f ").
                    Append(index1).Append(" ").
                    Append(index2).Append(" ").
                    Append(index3).Append("\n");
                }
            }

            sb.Append("\n# End Group v3d.csg").Append("\n");

            return sb;
        }

        /// <summary>
        /// Returns this csg in OBJ string format.
        /// </summary>
        /// 
        /// <returns>this csg in OBJ string format</returns>
        /// 
        public string toObjString()
        {
            StringBuilder sb = new StringBuilder();
            return toObjString(sb).ToString();
        }

        public CSG weighted(WeightFunction f)
        {
            return new Modifier(f).modified(this);
        }

        /// <summary>
        /// Returns a transformed copy of this CSG.
        /// </summary>
        /// 
        /// <param name="transform">the transform to apply</param>
        /// 
        /// <returns>a transformed copy of this CSG</returns>
        /// 
        public CSG transformed(CSharpVecMath.Transform transform)
        {

            if (polygons.Count == 0)
            {
                return clone();
            }

            List<Polygon> newpolygons = this.polygons.Select(
                    p => p.transformed(transform)).ToList();

            CSG result = CSG.fromPolygons(newpolygons).optimization(getOptType());

            result.storage = storage;

            return result;
        }

        // TODO finish experiment (20.7.2014)
        public MeshContainer toWPFMesh()
        {

            return toWPFMeshSimple();

            // TODO test obj approach with multiple materials
            //        try {
            //            ObjImporter importer = new ObjImporter(toObj());
            //
            //            List<Mesh> meshes = new ArrayList<>(importer.getMeshCollection());
            //            return new MeshContainer(getBounds().getMin(), getBounds().getMax(),
            //                    meshes, new ArrayList<>(importer.getMaterialCollection()));
            //        } catch (IOException ex) {
            //            Logger.getLogger(CSG.class.getName()).log(Level.SEVERE, null, ex);
            //        }
            //        // we have no backup strategy for broken streams :(
            //        return null;
        }

        /// <summary>
        /// Returns the CSG as JavaFX triangle mesh.
        /// </summary>
        /// 
        /// <returns>the CSG as JavaFX triangle mesh</returns>
        /// 
        public MeshContainer toWPFMeshSimple()
        {
            MeshGeometry3D mesh = new MeshGeometry3D();


            double minX = Double.PositiveInfinity;
            double minY = Double.PositiveInfinity;
            double minZ = Double.PositiveInfinity;

            double maxX = Double.NegativeInfinity;
            double maxY = Double.NegativeInfinity;
            double maxZ = Double.NegativeInfinity;

            int counter = 0;
            foreach (Polygon p in getPolygons())
            {
                if (p.vertices.Count >= 3)
                {

                    // TODO: improve the triangulation?
                    //
                    // JavaOne requires triangular polygons.
                    // If our polygon has more vertices, create
                    // multiple triangles:
                    Vertex firstVertex = p.vertices[0];
                    for (int i = 0; i < p.vertices.Count - 2; i++)
                    {

                        if (firstVertex.pos.x() < minX)
                        {
                            minX = firstVertex.pos.x();
                        }
                        if (firstVertex.pos.y() < minY)
                        {
                            minY = firstVertex.pos.y();
                        }
                        if (firstVertex.pos.z() < minZ)
                        {
                            minZ = firstVertex.pos.z();
                        }

                        if (firstVertex.pos.x() > maxX)
                        {
                            maxX = firstVertex.pos.x();
                        }
                        if (firstVertex.pos.y() > maxY)
                        {
                            maxY = firstVertex.pos.y();
                        }
                        if (firstVertex.pos.z() > maxZ)
                        {
                            maxZ = firstVertex.pos.z();
                        }

                        mesh.Positions.Add(new Point3D(
                                (float)firstVertex.pos.x(),
                                (float)firstVertex.pos.y(),
                                (float)firstVertex.pos.z()));

                        //mesh.getTexCoords().AddRange(0); // texture (not covered)
                        //mesh.getTexCoords().AddRange(0);

                        Vertex secondVertex = p.vertices[i + 1];

                        if (secondVertex.pos.x() < minX)
                        {
                            minX = secondVertex.pos.x();
                        }
                        if (secondVertex.pos.y() < minY)
                        {
                            minY = secondVertex.pos.y();
                        }
                        if (secondVertex.pos.z() < minZ)
                        {
                            minZ = secondVertex.pos.z();
                        }

                        if (secondVertex.pos.x() > maxX)
                        {
                            maxX = firstVertex.pos.x();
                        }
                        if (secondVertex.pos.y() > maxY)
                        {
                            maxY = firstVertex.pos.y();
                        }
                        if (secondVertex.pos.z() > maxZ)
                        {
                            maxZ = firstVertex.pos.z();
                        }

                        mesh.Positions.Add(new Point3D(
                                (float)secondVertex.pos.x(),
                                (float)secondVertex.pos.y(),
                                (float)secondVertex.pos.z()));

                        //mesh.getTexCoords().AddRange(0); // texture (not covered)
                        //mesh.getTexCoords().AddRange(0);

                        Vertex thirdVertex = p.vertices[i + 2];

                        mesh.Positions.Add(new Point3D(
                                (float)thirdVertex.pos.x(),
                                (float)thirdVertex.pos.y(),
                                (float)thirdVertex.pos.z()));

                        if (thirdVertex.pos.x() < minX)
                        {
                            minX = thirdVertex.pos.x();
                        }
                        if (thirdVertex.pos.y() < minY)
                        {
                            minY = thirdVertex.pos.y();
                        }
                        if (thirdVertex.pos.z() < minZ)
                        {
                            minZ = thirdVertex.pos.z();
                        }

                        if (thirdVertex.pos.x() > maxX)
                        {
                            maxX = firstVertex.pos.x();
                        }
                        if (thirdVertex.pos.y() > maxY)
                        {
                            maxY = firstVertex.pos.y();
                        }
                        if (thirdVertex.pos.z() > maxZ)
                        {
                            maxZ = firstVertex.pos.z();
                        }

                        //mesh.getTexCoords().AddRange(0); // texture (not covered)
                        //mesh.getTexCoords().AddRange(0);

                        mesh.TriangleIndices.Add(counter); // first vertex
                        mesh.TriangleIndices.Add(counter + 1);
                        mesh.TriangleIndices.Add(counter + 2);
                        counter += 3;
                    } // end for
                } // end if #verts >= 3

            } // end for polygon

            return new MeshContainer(
                    Vector3d.xyz(minX, minY, minZ),
                    Vector3d.xyz(maxX, maxY, maxZ), mesh);
        }



        /// <summary>
        /// Computes and returns the volume of this CSG based on a triangulated version
        /// of the internal mesh.
        /// </summary>
        /// <returns>volume of this csg</returns>
        /// 
        public double computeVolume()
        {
            if (getPolygons().Count == 0) return 0;

            // triangulate polygons (parallel for larger meshes)
            List<Polygon> triangles;
            var selector = new Func<Polygon, List<Polygon>>(poly => poly.toTriangles());
            if (getPolygons().Count > 200)
            {
                triangles = polygons.AsParallel().SelectMany(selector).ToList();
            }
            else
            {
                triangles = polygons.SelectMany(selector).ToList();
            }

            // compute sum over signed volumes of triangles
            // we use parallel streams for larger meshes
            // see http://chenlab.ece.cornell.edu/Publication/Cha/icip01_Cha.pdf
            double volume = 0.0;
            var signedVolume = new Func<Polygon, double>(tri =>
            {
                IVector3d p1 = tri.vertices[0].pos;
                IVector3d p2 = tri.vertices[1].pos;
                IVector3d p3 = tri.vertices[2].pos;

                return p1.dot(p2.crossed(p3)) / 6.0;
            });
            if (triangles.Count > 200)
            {
                volume = triangles.AsParallel().Select(signedVolume).Sum();
            }
            else
            {
                volume = triangles.Select(signedVolume).Sum();
            }

            return Math.Abs(volume);
        }

    }
}
