using Liru3D.Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System.Collections.Generic;

namespace Liru3D.Models.Data
{
    public class SkinnedMeshReader : ContentTypeReader<SkinnedModel>
    {
        protected override SkinnedModel Read(ContentReader input, SkinnedModel existingInstance)
        {
            // Read the number of meshes and create a new array to store them.
            int meshCount = input.ReadInt32();
            SkinnedMeshData[] meshes = new SkinnedMeshData[meshCount];

            // Load each mesh.
            for (int meshIndex = 0; meshIndex < meshCount; meshIndex++)
            {
                // Read the name.
                string name = input.ReadString();

                // Read the vertices.
                int vertexCount = input.ReadInt32();
                SkinnedVertex[] vertices = new SkinnedVertex[vertexCount];
                for (int vertexIndex = 0; vertexIndex < vertexCount; vertexIndex++)
                {
                    // Read each piece of data for the vertex.
                    SkinnedVertex vertex = new SkinnedVertex
                    {
                        Position = input.ReadVector3(),
                        Normal = input.ReadVector3(),
                        UV = input.ReadVector2(),
                        BlendIndices = new Byte4(input.ReadVector4()),
                        BlendWeights = input.ReadVector4()
                    };

                    // Set the vertex.
                    vertices[vertexIndex] = vertex;
                }

                // Read the indices.
                int indexCount = input.ReadInt32();
                int[] indices = new int[indexCount];
                for (int indexIndex = 0; indexIndex < indexCount; indexIndex++)
                    indices[indexIndex] = input.ReadInt32();

                // Add the mesh to the array.
                meshes[meshIndex] = new SkinnedMeshData(name, vertices, indices);
            }

            // Read the number of animations and create an array to hold them all.
            int animationCount = input.ReadInt32();
            List<Animation> animations = new List<Animation>(animationCount);

            // Load each animation.
            for (int animationIndex = 0; animationIndex < animationCount; animationIndex++)
            {
                // Read the name.
                string name = input.ReadString();

                // Read the timing data.
                int ticksPerSecond = input.ReadInt32();
                int durationInTicks = input.ReadInt32();

                // Read the number of channels and create a collection to hold them all.
                int channelCount = input.ReadInt32();
                Dictionary<string, BoneChannel> channels = new Dictionary<string, BoneChannel>(channelCount);

                // Load each channel.
                for (int channelIndex = 0; channelIndex < channelCount; channelIndex++)
                {
                    // Read the name of the channel.
                    string channelName = input.ReadString();

                    // Read each component of the channel.
                    int scalesCount = input.ReadInt32();
                    List<Keyframe<Vector3>> scaleFrames = new List<Keyframe<Vector3>>(scalesCount);
                    for (int scalesIndex = 0; scalesIndex < scalesCount; scalesIndex++)
                        scaleFrames.Add(new Keyframe<Vector3>(input.ReadInt32(), input.ReadInt32(), input.ReadVector3()));

                    int rotationsCount = input.ReadInt32();
                    List<Keyframe<Quaternion>> rotationFrames = new List<Keyframe<Quaternion>>(rotationsCount);
                    for (int rotationsIndex = 0; rotationsIndex < rotationsCount; rotationsIndex++)
                        rotationFrames.Add(new Keyframe<Quaternion>(input.ReadInt32(), input.ReadInt32(), input.ReadQuaternion()));

                    int positionsCount = input.ReadInt32();
                    List<Keyframe<Vector3>> positionFrames = new List<Keyframe<Vector3>>(positionsCount);
                    for (int positionsIndex = 0; positionsIndex < positionsCount; positionsIndex++)
                        positionFrames.Add(new Keyframe<Vector3>(input.ReadInt32(), input.ReadInt32(), input.ReadVector3()));

                    // Create and add the channel to the collection.
                    channels.Add(channelName, new BoneChannel(channelName, scaleFrames, rotationFrames, positionFrames));
                }

                // Create an animation from the channel data and add it to the array.
                animations.Add(new Animation(name, ticksPerSecond, durationInTicks, channels));
            }

            // Read the bones.
            int boneCount = input.ReadInt32();
            BoneData[] bones = new BoneData[boneCount];
            for (int boneIndex = 0; boneIndex < boneCount; boneIndex++)
            {
                // Read each piece of data for the bone.
                BoneData boneData = new BoneData()
                {
                    Name = input.ReadString(),
                    Index = input.ReadInt32(),
                    ParentIndex = input.ReadInt32(),
                    Offset = input.ReadMatrix(),
                    LocalTransform = input.ReadMatrix()
                };

                // Set the bone.
                bones[boneIndex] = boneData;
            }

            // Create and return the model data with the loaded meshes, animations, and bones.
            return SkinnedModel.CreateFrom(input.GetGraphicsDevice(), new SkinnedModelData(meshes, animations, bones));
        }
    }
}
