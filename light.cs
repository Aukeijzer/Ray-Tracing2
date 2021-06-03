using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
    //Class for the light objects, used to calculate shadows and reflections.
    public class Light
    {
        public Vector3 pos;
        public float i;

        public Light(Vector3 position, float intensity)
        {
            pos = position;
            i = intensity;
        }
    }
}
