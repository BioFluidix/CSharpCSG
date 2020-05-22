using System;
using CSharpCSG;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CCSGTest
{
    

    [TestClass]
    public class VolumeTest
    {
        private const double EPSILON = 1e-16;

        [TestMethod]
        public void TestVolume()
        {
            {
                // volume of empty CSG object is 0
                double emptyVolume = CSG.fromPolygons(new Polygon[0]).computeVolume();
                Assert.AreEqual(emptyVolume, 0, EPSILON, "Empty volume calculation failed");
            }

            {
                // volume of unit cube is 1 unit^3 
                double volumeUnitCube = new Cube(1.0).toCSG().computeVolume();
                Assert.AreEqual(1.0, volumeUnitCube, EPSILON, "Unit cube volume calculation failed");
            }

            {
                // volume of cube is w*h*d unit^3 
                double w = 30.65;
                double h = 24.17;
                double d = 75.3;
                double volumeBox = new Cube(w, h, d).toCSG().computeVolume();
                Assert.AreEqual(w * h * d, volumeBox, EPSILON, $"Box volume calculation failed delta:{(w * h * d) - volumeBox}");
            }

            {
                // volume of sphere is (4*PI*r^3)/3.0 unit^3
                double r = 3.4;

                // bad approximation
                double volumeSphere1 = new Sphere(r, 32, 16).toCSG().computeVolume();
                Assert.AreEqual((4.0 * Math.PI * r * r * r) / 3.0, volumeSphere1, 10.0);

                // better approximation
                double volumeSphere2 = new Sphere(r, 1024, 512).toCSG().computeVolume();
                Assert.AreEqual((4.0 * Math.PI * r * r * r) / 3.0, volumeSphere2, 1e-2);
            }

            {
                // volume of cylinder is PI*r^2*h unit^3
                double r = 5.9;
                double h = 2.1;

                // bad approximation
                double volumeCylinder1 = new Cylinder(r, h, 16).toCSG().computeVolume();
                Assert.AreEqual(Math.PI * r * r * h, volumeCylinder1, 10);

                // better approximation
                double volumeCylinder2 = new Cylinder(r, h, 1024).toCSG().computeVolume();
                Assert.AreEqual(Math.PI * r * r * h, volumeCylinder2, 1e-2);
            }

        }
    }
}
