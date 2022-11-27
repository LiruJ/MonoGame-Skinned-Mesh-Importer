using Assimp;
using Liru3D.Animations;
using Liru3D.Models;
using Liru3D.Models.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Collections.Generic;

namespace SkinnedMeshImporter
{
    [ContentProcessor(DisplayName = "SkinnedMeshProcessor - Liru3D")]
    public class SkinnedMeshProcessor : ContentProcessor<Scene, SkinnedModelData>
    {
        #region Settings
        /// <summary> Sometimes values have a slight error due to the constant number crunching. For example, the scale might go from 1 to 0.99999994. This option allows this error to be rounded away. 0 disables rounding. </summary>
        public int KeyframeDecimalPlaceRounding { get; set; } = 5;
        #endregion

        #region Process Functions
        public override SkinnedModelData Process(Scene input, ContentProcessorContext context)
        {
            // Build the armature for the scene. This takes every bone and puts them in a nicely ordered data structure that makes everything else way easier.
            buildArmature(input, out armatureData armature);

            // Create a collection of meshes.
            SkinnedMeshData[] skinnedMeshData = new SkinnedMeshData[input.MeshCount];
            
            // Process each mesh in the scene.
            for (int meshIndex = 0; meshIndex < input.MeshCount; meshIndex++)
            {
                // Get the current mesh.
                Mesh mesh = input.Meshes[meshIndex];

                // Load the indices.
                int[] indices = mesh.GetIndices();

                // Create the vertex array.
                SkinnedVertex[] vertices = new SkinnedVertex[mesh.VertexCount];

                // Populate the non-bone data of the model.
                for (int vertexIndex = 0; vertexIndex < mesh.VertexCount; vertexIndex++)
                {
                    // Create and populate the vertex.
                    SkinnedVertex vertex = new SkinnedVertex
                    {
                        Position = mesh.Vertices[vertexIndex].ToMonoGameVector3(),
                        Normal = mesh.Normals[vertexIndex].ToMonoGameVector3(),
                        UV = mesh.TextureCoordinateChannels[0][vertexIndex].ToMonoGameVector2(),
                    };

                    // Set the vertex.
                    vertices[vertexIndex] = vertex;
                }

                // Populate the bone data.
                foreach (Assimp.Bone bone in mesh.Bones)
                {
                    // Get the index of the bone within the armature.
                    int boneIndex = armature.GetIndexOf(bone);

                    // Go over each vertex this bone influences and add the bone's weight and index to them.
                    for (int weightIndex = 0; weightIndex < bone.VertexWeightCount; weightIndex++)
                    {
                        // Get the current weight data.
                        VertexWeight vertexWeight = bone.VertexWeights[weightIndex];

                        // Add this bone's influence to the vertex.
                        vertices[vertexWeight.VertexID].SetNextWeight(boneIndex, vertexWeight.Weight);
                    }
                }

                // Create and save the mesh data.
                skinnedMeshData[meshIndex] = new SkinnedMeshData(mesh.Name, vertices, indices);
            }

            // Process each animation in the scene.
            List<Liru3D.Animations.Animation> animations = new List<Liru3D.Animations.Animation>(input.AnimationCount);
            for (int animationIndex = 0; animationIndex < input.AnimationCount; animationIndex++)
            {
                // Get the animation data.
                Assimp.Animation inputAnimation = input.Animations[animationIndex];

                // Create a new dictionary to hold each bone's animation channel.
                Dictionary<string, BoneChannel> channelsByBoneName = new Dictionary<string, BoneChannel>();

                // Go over each channel of the animation. A channel is essentially a series of movements that a single node has during the animation.
                for (int channelIndex = 0; channelIndex < inputAnimation.NodeAnimationChannelCount; channelIndex++)
                {
                    // Get the channel.
                    NodeAnimationChannel inputChannel = inputAnimation.NodeAnimationChannels[channelIndex];

                    // If the channel isn't for a bone, then ignore it.
                    // This may cause issues for non-bones that are animated, but it's a safe assumption that nobody's out there directly animating nodes.
                    if (!armature.BoneNodesByBoneName.TryGetValue(inputChannel.NodeName, out Node boneNode)) continue;

                    // If this channel's node's parent is not a bone, then its parent's transforms need to be applied to every animated value.
                    // This is because usually the armature itself is rotated 90 degrees so that Y is up, but since the armature is being disposed of, this needs to be baked into the data.
                    bool needsTransforming = !armature.BoneNodesByBoneName.ContainsKey(boneNode.Parent.Name);
                    Matrix parentTransform = needsTransforming ? boneNode.Parent.Transform.ToMonoGameMatrixTransposed() : Matrix.Identity;

                    // Create collections for each component of the channel.
                    List<Keyframe<Vector3>> scaleFrames = new List<Keyframe<Vector3>>(inputChannel.ScalingKeyCount);
                    List<Keyframe<Microsoft.Xna.Framework.Quaternion>> rotationFrames = new List<Keyframe<Microsoft.Xna.Framework.Quaternion>>(inputChannel.RotationKeyCount);
                    List<Keyframe<Vector3>> positionFrames = new List<Keyframe<Vector3>>(inputChannel.PositionKeyCount);

                    // Go over each scale frame.
                    for (int scaleFrameIndex = 0; scaleFrameIndex < inputChannel.ScalingKeyCount; scaleFrameIndex++)
                    {
                        // Get the assimp frame.
                        VectorKey scaleKey = inputChannel.ScalingKeys[scaleFrameIndex];

                        // Get the scale.
                        Vector3 scale = scaleKey.Value.ToMonoGameVector3();

                        // If this component needs to be transformed around the armature, do so.
                        if (needsTransforming) (Matrix.CreateScale(scale) * parentTransform).Decompose(out scale, out _, out _);

                        // If rounding should happen, do so.
                        if (KeyframeDecimalPlaceRounding > 0)
                        {
                            scale.X = (float)Math.Round(scale.X, KeyframeDecimalPlaceRounding);
                            scale.Y = (float)Math.Round(scale.Y, KeyframeDecimalPlaceRounding);
                            scale.Z = (float)Math.Round(scale.Z, KeyframeDecimalPlaceRounding);
                        }

                        // Add the frame.
                        scaleFrames.Add(new Keyframe<Vector3>(scaleFrameIndex, (int)Math.Round(scaleKey.Time), scale));
                    }

                    // Go over each rotation frame.
                    for (int rotationFrameIndex = 0; rotationFrameIndex < inputChannel.RotationKeyCount; rotationFrameIndex++)
                    {
                        // Get the assimp frame.
                        QuaternionKey quaternionKey = inputChannel.RotationKeys[rotationFrameIndex];

                        // Get the rotation.
                        Microsoft.Xna.Framework.Quaternion rotation = quaternionKey.Value.ToMonoGameQuaternion();

                        // If this component needs to be transformed around the armature, do so.
                        if (needsTransforming) (Matrix.CreateFromQuaternion(rotation) * parentTransform).Decompose(out _, out rotation, out _);

                        // If rounding should happen, do so.
                        if (KeyframeDecimalPlaceRounding > 0)
                        {
                            rotation.X = (float)Math.Round(rotation.X, KeyframeDecimalPlaceRounding);
                            rotation.Y = (float)Math.Round(rotation.Y, KeyframeDecimalPlaceRounding);
                            rotation.Z = (float)Math.Round(rotation.Z, KeyframeDecimalPlaceRounding);
                            rotation.W = (float)Math.Round(rotation.W, KeyframeDecimalPlaceRounding);
                        }

                        // Add the frame.
                        rotationFrames.Add(new Keyframe<Microsoft.Xna.Framework.Quaternion>(rotationFrameIndex, (int)Math.Round(quaternionKey.Time), rotation));
                    }

                    // Go over each position frame.
                    for (int positionFrameIndex = 0; positionFrameIndex < inputChannel.PositionKeyCount; positionFrameIndex++)
                    {
                        // Get the assimp frame.
                        VectorKey positionKey = inputChannel.PositionKeys[positionFrameIndex];

                        // Get the position.
                        Vector3 position = positionKey.Value.ToMonoGameVector3();

                        // If this component needs to be transformed around the armature, do so.
                        if (needsTransforming) (Matrix.CreateTranslation(position) * parentTransform).Decompose(out _, out _, out position);

                        // If rounding should happen, do so.
                        if (KeyframeDecimalPlaceRounding > 0)
                        {
                            position.X = (float)Math.Round(position.X, KeyframeDecimalPlaceRounding);
                            position.Y = (float)Math.Round(position.Y, KeyframeDecimalPlaceRounding);
                            position.Z = (float)Math.Round(position.Z, KeyframeDecimalPlaceRounding);
                        }

                        // Add the frame.
                        positionFrames.Add(new Keyframe<Vector3>(positionFrameIndex, (int)Math.Round(positionKey.Time), position));
                    }

                    // Create a channel for the bone and add it to the collection.
                    channelsByBoneName.Add(inputChannel.NodeName, new BoneChannel(inputChannel.NodeName, scaleFrames, rotationFrames, positionFrames));
                }

                // Process the name. Remove "Armature|" from it.
                string name = inputAnimation.Name.Remove(0, inputAnimation.Name.IndexOf('|') + 1);

                // Create and add the animation.
                animations.Add(new Liru3D.Animations.Animation(name, (int)Math.Round(inputAnimation.TicksPerSecond), (int)Math.Round(inputAnimation.DurationInTicks), channelsByBoneName));
            }

            //for (int i = 0; i < skinnedMeshData[1].VertexCount; i++)
            //{
            //    context.Logger.LogMessage("{0}", skinnedMeshData[1].Vertices[i].BlendIndices.ToVector4());
            //}

            return new SkinnedModelData(skinnedMeshData, animations, armature.Bones);
        }
        #endregion

        #region Armature Helpers
        private struct armatureData
        {
            #region Backing Fields
            private readonly List<BoneData> bones;

            private readonly Dictionary<string, Node> boneNodesByBoneName;

            private readonly Dictionary<string, Assimp.Bone> bonesByNodeName;
            #endregion

            #region Properties
            /// <summary> The collection of bone nodes keyed by bone name. </summary>
            public IReadOnlyDictionary<string, Node> BoneNodesByBoneName => boneNodesByBoneName;

            public IReadOnlyDictionary<string, Assimp.Bone> BonesByNodeName => bonesByNodeName;

            public IReadOnlyList<BoneData> Bones => bones;

            public Assimp.Bone RootBone { get; }

            public Node RootBoneNode { get; }
            #endregion

            #region Constructors
            public armatureData(Assimp.Bone rootBone, List<BoneData> bones, Dictionary<string, Node> boneNodesByBoneName, Dictionary<string, Assimp.Bone> bonesByNodeName)
            {
                RootBone = rootBone;
                RootBoneNode = boneNodesByBoneName[rootBone.Name];

                this.bones = bones;
                this.boneNodesByBoneName = boneNodesByBoneName;
                this.bonesByNodeName = bonesByNodeName;
            }
            #endregion

            #region Bone Functions
            public int GetIndexOf(Assimp.Bone bone) => bones.FindIndex((data) => data.Name == bone.Name);

            public int GetParentIndex(Assimp.Bone bone)
            {
                string parentName = BoneNodesByBoneName[bone.Name].Parent.Name;
                return bones.FindIndex((data) => data.Name == parentName);
            }
            #endregion
        }

        private void buildArmature(Scene scene, out armatureData armature)
        {
            List<Assimp.Bone> bones = new List<Assimp.Bone>();

            // Add the bones of each mesh to the collection.
            foreach (Mesh mesh in scene.Meshes)
                foreach (Assimp.Bone bone in mesh.Bones)
                    if (!bones.Contains(bone)) bones.Add(bone);

            // Create the raw collections to hold the data about the armature.
            List<BoneData> boneList = new List<BoneData>();
            Dictionary<string, Node> boneNodesByBoneName = new Dictionary<string, Node>();
            Dictionary<string, Assimp.Bone> bonesByNodeName = new Dictionary<string, Assimp.Bone>();

            // Populate the dictionaries, keying together bones and nodes.
            Assimp.Bone rootBone = null;
            populateArmatureDictionary(bones, boneNodesByBoneName, bonesByNodeName, scene.RootNode, ref rootBone);

            // Ensure a root bone exists.
            if (rootBone == null)
                throw new Exception("Cannot created skinned mesh of mesh with no bones!");

            // Create the armature with the references to the created collections.
            armature = new armatureData(rootBone, boneList, boneNodesByBoneName, bonesByNodeName);

            // Populate the bone list, this updates the underlying collection within the armature.
            int boneIndex = 0;
            populateArmatureBoneList(ref boneIndex, boneList, rootBone, armature);
        }

        private void populateArmatureBoneList(ref int boneIndex, List<BoneData> bones, Assimp.Bone currentBone, armatureData armature)
        {
            int parentIndex = armature.GetParentIndex(currentBone);
            Node boneNode = armature.BoneNodesByBoneName[currentBone.Name];

            bones.Add(new BoneData
            {
                Name = currentBone.Name,
                Index = boneIndex,
                ParentIndex = parentIndex,
                Offset = currentBone.OffsetMatrix.ToMonoGameMatrixTransposed(),
                LocalTransform = parentIndex >= 0
                    ? boneNode.Transform.ToMonoGameMatrixTransposed()
                    : (boneNode.Transform * boneNode.Parent.Transform).ToMonoGameMatrixTransposed()
            });

            boneIndex++;

            foreach (Node childBoneNode in boneNode.Children)
                populateArmatureBoneList(ref boneIndex, bones, armature.BonesByNodeName[childBoneNode.Name], armature);
        }

        private void populateArmatureDictionary(IReadOnlyCollection<Assimp.Bone> bones, Dictionary<string, Node> boneNodesByBoneName, Dictionary<string, Assimp.Bone> bonesByNodeName, Node currentNode, ref Assimp.Bone rootBone)
        {
            // Go over each child node in the given node.
            foreach (Node childNode in currentNode.Children)
            {
                // See if this node's name matches the name of any bone. If it does, then add it to the dictionary.
                foreach (Assimp.Bone bone in bones)
                    if (bone.Name == childNode.Name)
                    {
                        if (!boneNodesByBoneName.ContainsKey(bone.Name))
                        {
                            boneNodesByBoneName.Add(bone.Name, childNode);
                            bonesByNodeName.Add(bone.Name, bone);

                            // If this bone's parent node is not a bone, then this bone is the root bone.
                            if (!boneNodesByBoneName.ContainsKey(currentNode.Name))
                                rootBone = bone;
                        }
                        break;
                    }

                // If this node has children, recursively check them.
                populateArmatureDictionary(bones, boneNodesByBoneName, bonesByNodeName, childNode, ref rootBone);
            }
        }
        #endregion
    }
}
