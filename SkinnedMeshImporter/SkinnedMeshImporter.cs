using Assimp;
using Assimp.Configs;
using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;

namespace SkinnedMeshImporter
{
    [ContentImporter(".fbx", DisplayName = "SkinnedMeshImporter - Liru3D", DefaultProcessor = "SkinnedMeshProcessor")]
    public class SkinnedMeshImporter : ContentImporter<Scene>
    {
        private static readonly PostProcessSteps postProcessSteps =
            PostProcessSteps.FlipUVs                // currently need
                                                | PostProcessSteps.JoinIdenticalVertices  // optimizes indexed
                                                | PostProcessSteps.Triangulate            // precaution
                                                | PostProcessSteps.FindInvalidData        // sometimes normals export wrong (remove & replace:)
                                                | PostProcessSteps.GenerateSmoothNormals  // smooths normals after identical verts removed (or bad normals)
                                                | PostProcessSteps.ImproveCacheLocality   // possible better cache optimization                                        
                                                | PostProcessSteps.FixInFacingNormals     // doesn't work well with planes - turn off if some faces go dark                                       
                                                | PostProcessSteps.CalculateTangentSpace  // use if you'll probably be using normal mapping 
                                                | PostProcessSteps.GenerateUVCoords       // useful for non-uv-map export primitives                                                
                                                | PostProcessSteps.ValidateDataStructure
                                                | PostProcessSteps.FindInstances
                                                | PostProcessSteps.GlobalScale            // use with AI_CONFIG_GLOBAL_SCALE_FACTOR_KEY (if need)                                                
                                                | PostProcessSteps.FlipWindingOrder;       // (CCW to CW) Depends on your rasterizing setup (need clockwise to fix inside-out problem?)        

        private static readonly List<PropertyConfig> configurations = new List<PropertyConfig>()
        {
            new NoSkeletonMeshesConfig(true),      // true to disable dummy-skeleton mesh
            new FBXImportCamerasConfig(false),     // true would import cameras
            new SortByPrimitiveTypeConfig(PrimitiveType.Point | PrimitiveType.Line), // primitive types we should remove
            new VertexBoneWeightLimitConfig(4),    // max weights per vertex (4 is very common - our shader will use 4)
            new NormalSmoothingAngleConfig(66.0f), // if no normals, generate (threshold 66 degrees) 
            new FBXStrictModeConfig(false),        // true only for fbx-strict-mode
        };

        public override Scene Import(string filename, ContentImporterContext context)
        {
            // Ensure the file exists.
            if (!File.Exists(filename)) throw new FileNotFoundException("The skinned mesh model could not be found.", filename);
            
            // Create the context.
            AssimpContext assimpContext = new AssimpContext();
            foreach (PropertyConfig config in configurations)
                assimpContext.SetConfig(config);

            // Load the scene.
            Scene scene;
            try { scene = assimpContext.ImportFile(filename, postProcessSteps); }
            catch (AssimpException assimpException) { throw assimpException; }
            catch (Exception exception) { throw exception; }

            return scene;
        }
    }
}
