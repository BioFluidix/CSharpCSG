/*
 * MeshContainer.cs
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
using System.Windows.Media;
using System.Windows.Media.Media3D;
using CSharpVecMath;

namespace CSharpCSG
{
    /// 
    /// 
    /// author: Michael Hoffer &lt;info@michaelhoffer.de&gt;
    /// 
    public class MeshContainer
    {

        private readonly List<MeshGeometry3D> meshes;
        private readonly List<Material> materials;
        private readonly double width;
        private readonly double height;
        private readonly double depth;
        private readonly Bounds bounds;

        public MeshContainer(IVector3d min, IVector3d max, params MeshGeometry3D[] meshes)
            : this(min, max, meshes.ToList()) {}

        public MeshContainer(IVector3d min, IVector3d max, List<MeshGeometry3D> meshes)
        {
            this.meshes = meshes;
            this.materials = new List<Material>();
            this.bounds = new Bounds(min, max);
            this.width = bounds.getBounds().x();
            this.height = bounds.getBounds().y();
            this.depth = bounds.getBounds().z();

            DiffuseMaterial material = new DiffuseMaterial(new SolidColorBrush(Colors.Red));
            

            //PhongMaterial material = new PhongMaterial(Colors.Red);
            foreach (MeshGeometry3D mesh in meshes)
            {
                materials.Add(material);
            }
        }

        public MeshContainer(IVector3d min, IVector3d max, List<MeshGeometry3D> meshes, List<Material> materials)
        {
            this.meshes = meshes;
            this.materials = materials;
            this.bounds = new Bounds(min, max);
            this.width = bounds.getBounds().x();
            this.height = bounds.getBounds().y();
            this.depth = bounds.getBounds().z();

            if (materials.Count != meshes.Count)
            {
                throw new ArgumentException("Mesh list and Material list must not differ in size!");
            }

        }

        /// 
        /// <returns>the width</returns>
        /// 
        public double getWidth()
        {
            return width;
        }

        /// 
        /// <returns>the height</returns>
        /// 
        public double getHeight()
        {
            return height;
        }

        /// 
        /// <returns>the depth</returns>
        /// 
        public double getDepth()
        {
            return depth;
        }

        /// 
        /// <returns>the mesh</returns>
        /// 
        public List<MeshGeometry3D> getMeshes()
        {
            return meshes;
        }

        public string toString()
        {
            return bounds.ToString();
        }

        /// 
        /// <returns>the bounds</returns>
        /// 
        public Bounds getBounds()
        {
            return bounds;
        }

        /// 
        /// <returns>the materials</returns>
        /// 
        public List<Material> getMaterials()
        {
            return materials;
        }

        public List<GeometryModel3D> getAsMeshViews()
        {
            List<GeometryModel3D> result = new List<GeometryModel3D>(meshes.Count);

            for (int i = 0; i < meshes.Count; i++)
            {

                MeshGeometry3D mesh = meshes[i];
                Material mat = materials[i];

                GeometryModel3D view = new GeometryModel3D(mesh, mat);
                view.BackMaterial = mat;

                result.Add(view);
            }

            return result;
        }

        //    public javafx.scene.Node getAsInteractiveSubSceneNode() {
        //
        //        if (viewContainer != null) {
        //            return viewContainer;
        //        }
        //
        //        viewContainer = new Pane();
        //
        //        SubScene subScene = new SubScene(getRoot(), 100, 100, true, SceneAntialiasing.BALANCED);
        ////        subScene.setFill(Color.BLACK);
        //
        //        subScene.widthProperty().bind(viewContainer.widthProperty());
        //        subScene.heightProperty().bind(viewContainer.heightProperty());
        //
        //        PerspectiveCamera subSceneCamera = new PerspectiveCamera(false);
        //        subScene.setCamera(subSceneCamera);
        //
        //        viewContainer.getChildren().add(subScene);
        //
        //        getRoot().layoutXProperty().bind(viewContainer.widthProperty().divide(2));
        //        getRoot().layoutYProperty().bind(viewContainer.heightProperty().divide(2));
        //
        //        viewContainer.boundsInLocalProperty().addListener(
        //                (ObservableValue<? extends javafx.geometry.Bounds> ov, javafx.geometry.Bounds t, javafx.geometry.Bounds t1) -> {
        //                    setMeshScale(this, t1, getRoot());
        //                });
        //
        //        VFX3DUtil.addMouseBehavior(getRoot(), viewContainer, MouseButton.PRIMARY);
        //
        //        return viewContainer;
        //    }
        //
        //    private void setMeshScale(MeshContainer meshContainer, javafx.geometry.Bounds t1, final Group meshView) {
        //        double maxDim
        //                = Math.max(meshContainer.getWidth(),
        //                        Math.max(meshContainer.getHeight(), meshContainer.getDepth()));
        //
        //        double minContDim = Math.min(t1.getWidth(), t1.getHeight());
        //
        //        double scale = minContDim / (maxDim * 2);
        //
        //        //System.out.println("scale: " + scale + ", maxDim: " + maxDim + ", " + meshContainer);
        //        meshView.setScaleX(scale);
        //        meshView.setScaleY(scale);
        //        meshView.setScaleZ(scale);
        //    }
    }
}
