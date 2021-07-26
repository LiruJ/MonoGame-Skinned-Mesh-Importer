using Assimp;
using Microsoft.Xna.Framework;

// Prevents needing to type this whole thing out for certain types that have the same name between Assimp/MonoGame.
using Xna = Microsoft.Xna.Framework;

namespace SkinnedMeshImporter
{
    internal static class AssimpHelpers
    {
        #region Helper Functions
        /// <summary> Takes the given <paramref name="value"/>, if it is infinity or NaN, returns <c>0.0</c>, otherwise returns <paramref name="value"/>. </summary>
        /// <param name="value"> The value to correct. </param>
        /// <returns> <c>0.0</c> if the given <paramref name="value"/> is infinity or NaN, <paramref name="value"/> otherwise. </returns>
        private static float correctValue(float value) => float.IsNaN(value) || float.IsInfinity(value) ? 0.0f : value;

        public static Vector2 ToMonoGameVector2(this Vector2D vector2D) => new Vector2(vector2D.X, vector2D.Y);

        public static Vector2 ToMonoGameVector2(this Vector3D vector3D) => new Vector2(vector3D.X, vector3D.Y);

        public static Vector3 ToMonoGameVector3(this Vector3D vector3D) => new Vector3(vector3D.X, vector3D.Y, vector3D.Z);

        public static Xna.Quaternion ToMonoGameQuaternion(this Assimp.Quaternion quaternion) 
            => Xna.Quaternion.CreateFromRotationMatrix(quaternion.GetMatrix().ToMonoGameMatrixTransposed());

        public static Matrix ToMonoGameMatrixTransposed(this Matrix4x4 matrix) => Matrix.Transpose(new Matrix(
                correctValue(matrix.A1), correctValue(matrix.A2), correctValue(matrix.A3), correctValue(matrix.A4),
                correctValue(matrix.B1), correctValue(matrix.B2), correctValue(matrix.B3), correctValue(matrix.B4),
                correctValue(matrix.C1), correctValue(matrix.C2), correctValue(matrix.C3), correctValue(matrix.C4),
                correctValue(matrix.D1), correctValue(matrix.D2), correctValue(matrix.D3), correctValue(matrix.D4)));

        public static Matrix ToMonoGameMatrixTransposed(this Matrix3x3 matrix) => Matrix.Transpose(new Matrix(
            correctValue(matrix.A1), correctValue(matrix.A2), correctValue(matrix.A3), 0,
                correctValue(matrix.B1), correctValue(matrix.B2), correctValue(matrix.B3), 0,
                correctValue(matrix.C1), correctValue(matrix.C2), correctValue(matrix.C3), 0,
                0, 0, 0, 1));
        #endregion
    }
}
