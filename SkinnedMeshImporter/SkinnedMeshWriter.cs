using Liru3D.Animations;
using Liru3D.Models;
using Liru3D.Models.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace SkinnedMeshImporter
{
    [ContentTypeWriter]
    public class SkinnedMeshWriter : ContentTypeWriter<SkinnedModelData>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform) => typeof(SkinnedMeshReader).AssemblyQualifiedName;

        protected override void Write(ContentWriter output, SkinnedModelData value)
        {
            // Write the number of meshes.
            output.Write(value.MeshCount);

            // Write each mesh.
            foreach (SkinnedMeshData mesh in value.Meshes)
            {
                // Write the name.
                output.Write(mesh.Name);

                // Write the vertices.
                output.Write(mesh.VertexCount);
                foreach (SkinnedVertex vertex in mesh.Vertices)
                {
                    output.Write(vertex.Position);
                    output.Write(vertex.Normal);
                    output.Write(vertex.UV);
                    output.Write(vertex.BlendIndices.ToVector4());
                    output.Write(vertex.BlendWeights);
                }

                // Write the indices.
                output.Write(mesh.IndexCount);
                foreach (int index in mesh.Indices)
                    output.Write(index);
            }

            // Write the number of animations.
            output.Write(value.AnimationCount);

            // Write each animation.
            foreach (Animation animation in value.Animations)
            {
                // Write the name.
                output.Write(animation.Name);

                // Write the timing data.
                output.Write(animation.TicksPerSecond);
                output.Write(animation.DurationInTicks);

                // Write the number of channels.
                output.Write(animation.ChannelsByBoneName.Count);

                // Write each channel.
                foreach (BoneChannel channel in animation.ChannelsByBoneName.Values)
                {
                    // Write the name of the channel.
                    output.Write(channel.BoneName);

                    // Write each component of the channel.
                    output.Write(channel.Scales.Count);
                    foreach (Keyframe<Vector3> keyframe in channel.Scales.Keyframes)
                    {
                        output.Write(keyframe.Index);
                        output.Write(keyframe.TickTime);
                        output.Write(keyframe.Value);
                    }

                    output.Write(channel.Rotations.Count);
                    foreach (Keyframe<Quaternion> keyframe in channel.Rotations.Keyframes)
                    {
                        output.Write(keyframe.Index);
                        output.Write(keyframe.TickTime);
                        output.Write(keyframe.Value);
                    }

                    output.Write(channel.Positions.Count);
                    foreach (Keyframe<Vector3> keyframe in channel.Positions.Keyframes)
                    {
                        output.Write(keyframe.Index);
                        output.Write(keyframe.TickTime);
                        output.Write(keyframe.Value);
                    }
                }
            }

            // Write the bones.
            output.Write(value.BoneCount);
            foreach (BoneData boneData in value.Bones)
            {
                output.Write(boneData.Name);
                output.Write(boneData.Index);
                output.Write(boneData.ParentIndex);
                output.Write(boneData.Offset);
                output.Write(boneData.LocalTransform);
            }
        }
    }
}
