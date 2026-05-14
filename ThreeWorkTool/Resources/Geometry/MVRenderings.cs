using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using ThreeWorkTool.Resources.Geometry;
using ThreeWorkTool.Resources.Wrappers.ModelNodes;

namespace ThreeWorkTool.Resources.Geometry
{
    //Based on the Checkerboard class which means it has its own shader.
    public class Sphere : IDisposable
    {
        private int vao, vbo, ebo;
        private int shaderProgram;
        private int indexCount;
        private bool Disposed = false;
        private int SplineShader;

        //For Splines.
        public Color4 SplineColor { get; set; } = new Color4(0.0f, 0.0f, 0.0f, 1f);
        //public float SplineScale { get; set; } = 1.2f;
        public bool Spline { get; set; } = true;
        public float SplineWidth { get; set; } = 0.03f;
        public float Radius { get; set; } = 1.0f;
        public int Stacks { get; set; } = 16;
        public int Slices { get; set; } = 16;
        public Color4 Color { get; set; } = new Color4(0.4f, 0.9f, 0.0f, 1.0f);
        public bool Wireframe { get; set; } = false;

        //Spline needs its own shader.
        private static readonly string SplineVertSrc = @"
        #version 330 core
        layout(location = 0) in vec3 aPosition;

        uniform mat4 uModel;
        uniform mat4 uView;
        uniform mat4 uProjection;
        uniform float uSplineWidth;

        void main()
        {
            vec4 clipPos = uProjection * uView * uModel * vec4(aPosition, 1.0);

            // Expand along screen-space normal for consistent pixel width
            vec4 clipNormal = uProjection * uView * uModel * vec4(normalize(aPosition), 0.0);
            vec2 screenNormal = normalize(clipNormal.xy);
            clipPos.xy += screenNormal * uSplineWidth * clipPos.w;

            gl_Position = clipPos;
        }";

        private static readonly string SplineFragSrc = @"
        #version 330 core
        uniform vec4 uColor;
        out vec4 FragColor;

        void main()
        {
            FragColor = uColor;
        }";

        //This will have its own shader independent of anything else.
        private static readonly string VertexShaderSrc = @"
        #version 330 core
        layout(location = 0) in vec3 aPosition;

        uniform mat4 uModel;
        uniform mat4 uView;
        uniform mat4 uProjection;

        void main()
        {
            gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
        }";

        private static readonly string FragmentShaderSrc = @"
        #version 330 core
        uniform vec4 uColor;
        out vec4 FragColor;

        void main()
        {
            FragColor = uColor;
        }";



        public void Load()
        {
            shaderProgram = BuildShader(VertexShaderSrc, FragmentShaderSrc);
            SplineShader = BuildShader(SplineVertSrc, SplineFragSrc);
            //shaderProgram = BuildShader(VertexShaderSrc, FragmentShaderSrc);
            BuildMesh();
        }

        private void BuildMesh()
        {
            var verts = new List<float>();
            var indices = new List<int>();

            // Generate vertices (UV sphere)
            for (int stack = 0; stack <= Stacks; stack++)
            {
                float phi = MathHelper.Pi * stack / Stacks; // 0 to PI
                for (int slice = 0; slice <= Slices; slice++)
                {
                    float theta = 2f * MathHelper.Pi * slice / Slices; // 0 to 2PI

                    float x = Radius * (float)(Math.Sin(phi) * Math.Cos(theta));
                    float y = Radius * (float)Math.Cos(phi);
                    float z = Radius * (float)(Math.Sin(phi) * Math.Sin(theta));

                    verts.Add(x);
                    verts.Add(y);
                    verts.Add(z);
                }
            }

            // Generate indices
            for (int stack = 0; stack < Stacks; stack++)
            {
                for (int slice = 0; slice < Slices; slice++)
                {
                    int a = stack * (Slices + 1) + slice;
                    int b = (stack + 1) * (Slices + 1) + slice;

                    // Two triangles per quad
                    indices.Add(a);
                    indices.Add(b);
                    indices.Add(a + 1);

                    indices.Add(b);
                    indices.Add(b + 1);
                    indices.Add(a + 1);
                }
            }

            indexCount = indices.Count;

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            ebo = GL.GenBuffer();

            GL.BindVertexArray(vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer,
                verts.Count * sizeof(float),
                verts.ToArray(),
                BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer,
                indices.Count * sizeof(int),
                indices.ToArray(),
                BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);
        }

        public void Render(Matrix4 model, Matrix4 view, Matrix4 projection)
        {
            //We draw the Spline first.
            //Matrix4 SplineModel = Matrix4.CreateScale(SplineScale) * model;
            //GL.UseProgram(shaderProgram);
            GL.UseProgram(SplineShader);

            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "uModel"), false, ref model);
            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "uView"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "uProjection"), false, ref projection);
            GL.Uniform4(GL.GetUniformLocation(shaderProgram, "uColor"), SplineColor);
            GL.Uniform1(GL.GetUniformLocation(SplineShader, "uSplineWidth"), SplineWidth);

            //We change it's culling mode so it displays properly.
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front);

            //GL.Enable(EnableCap.PolygonOffsetFill);
            //GL.PolygonOffset(1f, 1f);

            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            //Restores original culling settings for actual sphere.
            //GL.Disable(EnableCap.PolygonOffsetFill);
            GL.Disable(EnableCap.CullFace);

            //This is for the Bone Sphere itself.
            GL.UseProgram(shaderProgram);

            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "uModel"), false, ref model);
            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "uView"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "uProjection"), false, ref projection);
            GL.Uniform4(GL.GetUniformLocation(shaderProgram, "uColor"), Color.R, Color.G, Color.B, Color.A);

            //if (Wireframe)
            //    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            //if (Wireframe)
            //    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill); // restore
        }

        private int BuildShader(string VertexShaderSrc, string FragmentShaderSrc)
        {
            int vert = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vert, VertexShaderSrc);
            GL.CompileShader(vert);
            System.Diagnostics.Debug.WriteLine($"Vert shader log: {GL.GetShaderInfoLog(vert)}");

            int frag = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(frag, FragmentShaderSrc);
            GL.CompileShader(frag);
            System.Diagnostics.Debug.WriteLine($"Frag shader log: {GL.GetShaderInfoLog(frag)}");

            int prog = GL.CreateProgram();
            GL.AttachShader(prog, vert);
            GL.AttachShader(prog, frag);
            GL.LinkProgram(prog);
            System.Diagnostics.Debug.WriteLine($"Program link log: {GL.GetProgramInfoLog(prog)}");

            GL.DeleteShader(vert);
            GL.DeleteShader(frag);
            return prog;
        }

        public void Dispose()
        {

            //Meant to avoid double disposing.
            if (Disposed)
            {
                return;
            }
            Disposed = true;

            GL.DeleteBuffer(vbo);
            GL.DeleteBuffer(ebo);
            GL.DeleteVertexArray(vao);
            GL.DeleteProgram(shaderProgram);
            GL.DeleteProgram(SplineShader);
            GC.SuppressFinalize(this);
        }
    }

    public class BoneRectangle
    {
        private int vao, vbo, ebo;
        private int shaderProgram;
        //private int indexCount;
        private bool Disposed = false;
        public Vector3 StartPos, EndPos;
        public Color4 LineColor { get; set; } = new Color4(0.1f, 0.05f, 0.05f, 1.0f);
        public float Width { get; set; } = 0.8f;


        //This will have its own shader independent of anything else.
        private static readonly string VertexShaderSrc = @"
        #version 330 core
        layout(location = 0) in vec3 aPosition;

        uniform mat4 uView;
        uniform mat4 uProjection;

        void main()
        {
            gl_Position = uProjection * uView * vec4(aPosition, 1.0);
        }";

        private static readonly string FragmentShaderSrc = @"
        #version 330 core
        uniform vec4 uColor;
        out vec4 FragColor;

        void main()
        {
            FragColor = uColor;
        }";

        public void Load()
        {

            shaderProgram = BuildShader(VertexShaderSrc, FragmentShaderSrc);

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            ebo = GL.GenBuffer();

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

            //This has 6 verts * 3 Floats, updated on a frame basis.
            GL.BufferData(BufferTarget.ArrayBuffer, 6 * 3 * sizeof(float), IntPtr.Zero, BufferUsageHint.DynamicDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);

        }

        public void Render(Vector3 startPos, Vector3 endPos, Vector3 cameraPos, Matrix4 view, Matrix4 projection)
        {
            // Perpendicular to the bone's direction, but also camera facing.
            Vector3 dir = Vector3.Normalize(endPos - startPos);
            Vector3 toCamera = Vector3.Normalize(cameraPos - (startPos + endPos) * 0.5f);
            Vector3 perp = Vector3.Normalize(Vector3.Cross(dir, toCamera)) * (Width * 0.5f);

            //Expands the endpoints into a quad with 2 triangles and 6 vertices.
            Vector3 a = startPos + perp;
            Vector3 b = startPos - perp;
            Vector3 c = endPos + perp;
            Vector3 d = endPos - perp;

            float[] verts =
            {
                a.X, a.Y, a.Z,
                b.X, b.Y, b.Z,
                c.X, c.Y, c.Z,

                b.X, b.Y, b.Z,
                d.X, d.Y, d.Z,
                c.X, c.Y, c.Z,
            };

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, verts.Length * sizeof(float), verts);

            GL.UseProgram(shaderProgram);
            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "uView"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "uProjection"), false, ref projection);
            GL.Uniform4(GL.GetUniformLocation(shaderProgram, "uColor"), LineColor.R, LineColor.G, LineColor.B, LineColor.A);

            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindVertexArray(0);
        }

        private int BuildShader(string VertexShaderSrc, string FragmentShaderSrc)
        {
            int vert = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vert, VertexShaderSrc);
            GL.CompileShader(vert);
            System.Diagnostics.Debug.WriteLine($"Vert shader log: {GL.GetShaderInfoLog(vert)}");

            int frag = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(frag, FragmentShaderSrc);
            GL.CompileShader(frag);
            System.Diagnostics.Debug.WriteLine($"Frag shader log: {GL.GetShaderInfoLog(frag)}");

            int prog = GL.CreateProgram();
            GL.AttachShader(prog, vert);
            GL.AttachShader(prog, frag);
            GL.LinkProgram(prog);
            System.Diagnostics.Debug.WriteLine($"Program link log: {GL.GetProgramInfoLog(prog)}");

            GL.DeleteShader(vert);
            GL.DeleteShader(frag);
            return prog;
        }

        public void Dispose()
        {

            //Meant to avoid double disposing.
            if (Disposed)
            {
                return;
            }
            Disposed = true;

            GL.DeleteBuffer(vbo);
            //GL.DeleteBuffer(ebo);
            GL.DeleteVertexArray(vao);
            GL.DeleteProgram(shaderProgram);
            GC.SuppressFinalize(this);

        }

    }

    public class MVRenderings
    {

        public void ShapesForBones(List<ModelBoneEntry> Joints, TheRenderer theRenderer, FrmModelViewer modelViewer)
        {

            for (int j = 0; j < Joints.Count; j++)
            {
                //Sphere.Render(Joints[j].);


            }



        }

    }
}
