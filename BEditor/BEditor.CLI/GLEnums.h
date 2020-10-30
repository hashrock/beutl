#pragma once

namespace BEditor {
	namespace CLI {
		namespace Graphics {
			public enum class TextureParameterName {
                TextureWidth = 4096,
                TextureHeight = 4097,
                TextureComponents = 4099,
                TextureInternalFormat = 4099,
                TextureBorderColor = 4100,
                TextureBorderColorNv = 4100,
                TextureBorder = 4101,
                TextureMagFilter = 10240,
                TextureMinFilter = 10241,
                TextureWrapS = 10242,
                TextureWrapT = 10243,
                TextureRedSize = 32860,
                TextureGreenSize = 32861,
                TextureBlueSize = 32862,
                TextureAlphaSize = 32863,
                TextureLuminanceSize = 32864,
                TextureIntensitySize = 32865,
                TexturePriority = 32870,
                TexturePriorityExt = 32870,
                TextureResident = 32871,
                TextureDepth = 32881,
                TextureDepthExt = 32881,
                TextureWrapR = 32882,
                TextureWrapRExt = 32882,
                TextureWrapROes = 32882,
                DetailTextureLevelSgis = 32922,
                DetailTextureModeSgis = 32923,
                DetailTextureFuncPointsSgis = 32924,
                SharpenTextureFuncPointsSgis = 32944,
                ShadowAmbientSgix = 32959,
                TextureCompareFailValue = 32959,
                DualTextureSelectSgis = 33060,
                QuadTextureSelectSgis = 33061,
                ClampToBorder = 33069,
                ClampToEdge = 33071,
                Texture4DsizeSgis = 33078,
                TextureWrapQSgis = 33079,
                TextureMinLod = 33082,
                TextureMinLodSgis = 33082,
                TextureMaxLod = 33083,
                TextureMaxLodSgis = 33083,
                TextureBaseLevel = 33084,
                TextureBaseLevelSgis = 33084,
                TextureMaxLevel = 33085,
                TextureMaxLevelSgis = 33085,
                TextureFilter4SizeSgis = 33095,
                TextureClipmapCenterSgix = 33137,
                TextureClipmapFrameSgix = 33138,
                TextureClipmapOffsetSgix = 33139,
                TextureClipmapVirtualDepthSgix = 33140,
                TextureClipmapLodOffsetSgix = 33141,
                TextureClipmapDepthSgix = 33142,
                PostTextureFilterBiasSgix = 33145,
                PostTextureFilterScaleSgix = 33146,
                TextureLodBiasSSgix = 33166,
                TextureLodBiasTSgix = 33167,
                TextureLodBiasRSgix = 33168,
                GenerateMipmap = 33169,
                GenerateMipmapSgis = 33169,
                TextureCompareSgix = 33178,
                TextureCompareOperatorSgix = 33179,
                TextureLequalRSgix = 33180,
                TextureGequalRSgix = 33181,
                TextureMaxClampSSgix = 33641,
                TextureMaxClampTSgix = 33642,
                TextureMaxClampRSgix = 33643,
                TextureLodBias = 34049,
                DepthTextureMode = 34891,
                TextureCompareMode = 34892,
                TextureCompareFunc = 34893,
                TextureSwizzleR = 36418,
                TextureSwizzleG = 36419,
                TextureSwizzleB = 36420,
                TextureSwizzleA = 36421,
                TextureSwizzleRgba = 36422,
                DepthStencilTextureMode = 37098,
                TextureTilingExt = 38272
			};
            public enum class TextureTarget {
                Texture1D = 3552,
                Texture2D = 3553,
                ProxyTexture1D = 32867,
                ProxyTexture1DExt = 32867,
                ProxyTexture2D = 32868,
                ProxyTexture2DExt = 32868,
                Texture3D = 32879,
                Texture3DExt = 32879,
                Texture3DOes = 32879,
                ProxyTexture3D = 32880,
                ProxyTexture3DExt = 32880,
                DetailTexture2DSgis = 32917,
                Texture4DSgis = 33076,
                ProxyTexture4DSgis = 33077,
                TextureRectangle = 34037,
                TextureRectangleArb = 34037,
                TextureRectangleNv = 34037,
                ProxyTextureRectangle = 34039,
                ProxyTextureRectangleArb = 34039,
                ProxyTextureRectangleNv = 34039,
                TextureCubeMap = 34067,
                TextureBindingCubeMap = 34068,
                TextureCubeMapPositiveX = 34069,
                TextureCubeMapNegativeX = 34070,
                TextureCubeMapPositiveY = 34071,
                TextureCubeMapNegativeY = 34072,
                TextureCubeMapPositiveZ = 34073,
                TextureCubeMapNegativeZ = 34074,
                ProxyTextureCubeMap = 34075,
                ProxyTextureCubeMapArb = 34075,
                ProxyTextureCubeMapExt = 34075,
                Texture1DArray = 35864,
                ProxyTexture1DArray = 35865,
                ProxyTexture1DArrayExt = 35865,
                Texture2DArray = 35866,
                ProxyTexture2DArray = 35867,
                ProxyTexture2DArrayExt = 35867,
                TextureBuffer = 35882,
                TextureCubeMapArray = 36873,
                TextureCubeMapArrayArb = 36873,
                TextureCubeMapArrayExt = 36873,
                TextureCubeMapArrayOes = 36873,
                ProxyTextureCubeMapArray = 36875,
                ProxyTextureCubeMapArrayArb = 36875,
                Texture2DMultisample = 37120,
                ProxyTexture2DMultisample = 37121,
                Texture2DMultisampleArray = 37122,
                ProxyTexture2DMultisampleArray = 37123
            };
            public enum class TextureMinFilter {
                Nearest = 9728,
                Linear = 9729,
                NearestMipmapNearest = 9984,
                LinearMipmapNearest = 9985,
                NearestMipmapLinear = 9986,
                LinearMipmapLinear = 9987,
                Filter4Sgis = 33094,
                LinearClipmapLinearSgix = 33136,
                PixelTexGenQCeilingSgix = 33156,
                PixelTexGenQRoundSgix = 33157,
                PixelTexGenQFloorSgix = 33158,
                NearestClipmapNearestSgix = 33869,
                NearestClipmapLinearSgix = 33870,
                LinearClipmapNearestSgix = 33871
            };
            public enum class TextureMagFilter {
                Nearest = 9728,
                Linear = 9729,
                LinearDetailSgis = 32919,
                LinearDetailAlphaSgis = 32920,
                LinearDetailColorSgis = 32921,
                LinearSharpenSgis = 32941,
                LinearSharpenAlphaSgis = 32942,
                LinearSharpenColorSgis = 32943,
                Filter4Sgis = 33094,
                PixelTexGenQCeilingSgix = 33156,
                PixelTexGenQRoundSgix = 33157,
                PixelTexGenQFloorSgix = 33158
            };
            public enum class TextureWrapMode {
                Clamp = 10496,
                Repeat = 10497,
                ClampToBorder = 33069,
                ClampToBorderArb = 33069,
                ClampToBorderNv = 33069,
                ClampToBorderSgis = 33069,
                ClampToEdge = 33071,
                ClampToEdgeSgis = 33071,
                MirroredRepeat = 33648
            };
            public enum class GenerateMipmapTarget {
                Texture1D = 3552,
                Texture2D = 3553,
                Texture3D = 32879,
                TextureCubeMap = 34067,
                Texture1DArray = 35864,
                Texture2DArray = 35866,
                TextureCubeMapArray = 36873,
                Texture2DMultisample = 37120,
                Texture2DMultisampleArray = 37122
            };
            public enum class BlendEquationMode {
                FuncAdd = 32774,
                Min = 32775,
                Max = 32776,
                FuncSubtract = 32778,
                FuncReverseSubtract = 32779
            };
            public enum class BlendingFactor {
                Zero = 0,
                One = 1,
                SrcColor = 768,
                OneMinusSrcColor = 769,
                SrcAlpha = 770,
                OneMinusSrcAlpha = 771,
                DstAlpha = 772,
                OneMinusDstAlpha = 773,
                DstColor = 774,
                OneMinusDstColor = 775,
                SrcAlphaSaturate = 776,
                ConstantColor = 32769,
                OneMinusConstantColor = 32770,
                ConstantAlpha = 32771,
                OneMinusConstantAlpha = 32772,
                Src1Alpha = 34185,
                Src1Color = 35065
            };
            public enum class EnableCap {
                PointSmooth = 2832,
                LineSmooth = 2848,
                LineStipple = 2852,
                PolygonSmooth = 2881,
                PolygonStipple = 2882,
                CullFace = 2884,
                Lighting = 2896,
                ColorMaterial = 2903,
                Fog = 2912,
                DepthTest = 2929,
                StencilTest = 2960,
                Normalize = 2977,
                AlphaTest = 3008,
                Dither = 3024,
                Blend = 3042,
                IndexLogicOp = 3057,
                ColorLogicOp = 3058,
                ScissorTest = 3089,
                TextureGenS = 3168,
                TextureGenT = 3169,
                TextureGenR = 3170,
                TextureGenQ = 3171,
                AutoNormal = 3456,
                Map1Color4 = 3472,
                Map1Index = 3473,
                Map1Normal = 3474,
                Map1TextureCoord1 = 3475,
                Map1TextureCoord2 = 3476,
                Map1TextureCoord3 = 3477,
                Map1TextureCoord4 = 3478,
                Map1Vertex3 = 3479,
                Map1Vertex4 = 3480,
                Map2Color4 = 3504,
                Map2Index = 3505,
                Map2Normal = 3506,
                Map2TextureCoord1 = 3507,
                Map2TextureCoord2 = 3508,
                Map2TextureCoord3 = 3509,
                Map2TextureCoord4 = 3510,
                Map2Vertex3 = 3511,
                Map2Vertex4 = 3512,
                Texture1D = 3552,
                Texture2D = 3553,
                PolygonOffsetPoint = 10753,
                PolygonOffsetLine = 10754,
                ClipDistance0 = 12288,
                ClipPlane0 = 12288,
                ClipDistance1 = 12289,
                ClipPlane1 = 12289,
                ClipDistance2 = 12290,
                ClipPlane2 = 12290,
                ClipDistance3 = 12291,
                ClipPlane3 = 12291,
                ClipDistance4 = 12292,
                ClipPlane4 = 12292,
                ClipDistance5 = 12293,
                ClipPlane5 = 12293,
                ClipDistance6 = 12294,
                ClipDistance7 = 12295,
                Light0 = 16384,
                Light1 = 16385,
                Light2 = 16386,
                Light3 = 16387,
                Light4 = 16388,
                Light5 = 16389,
                Light6 = 16390,
                Light7 = 16391,
                Convolution1D = 32784,
                Convolution1DExt = 32784,
                Convolution2D = 32785,
                Convolution2DExt = 32785,
                Separable2D = 32786,
                Separable2DExt = 32786,
                Histogram = 32804,
                HistogramExt = 32804,
                MinmaxExt = 32814,
                PolygonOffsetFill = 32823,
                RescaleNormal = 32826,
                RescaleNormalExt = 32826,
                Texture3DExt = 32879,
                VertexArray = 32884,
                NormalArray = 32885,
                ColorArray = 32886,
                IndexArray = 32887,
                TextureCoordArray = 32888,
                EdgeFlagArray = 32889,
                InterlaceSgix = 32916,
                Multisample = 32925,
                MultisampleSgis = 32925,
                SampleAlphaToCoverage = 32926,
                SampleAlphaToMaskSgis = 32926,
                SampleAlphaToOne = 32927,
                SampleAlphaToOneSgis = 32927,
                SampleCoverage = 32928,
                SampleMaskSgis = 32928,
                TextureColorTableSgi = 32956,
                ColorTable = 32976,
                ColorTableSgi = 32976,
                PostConvolutionColorTable = 32977,
                PostConvolutionColorTableSgi = 32977,
                PostColorMatrixColorTable = 32978,
                PostColorMatrixColorTableSgi = 32978,
                Texture4DSgis = 33076,
                PixelTexGenSgix = 33081,
                SpriteSgix = 33096,
                ReferencePlaneSgix = 33149,
                IrInstrument1Sgix = 33151,
                CalligraphicFragmentSgix = 33155,
                FramezoomSgix = 33163,
                FogOffsetSgix = 33176,
                SharedTexturePaletteExt = 33275,
                DebugOutputSynchronous = 33346,
                AsyncHistogramSgix = 33580,
                PixelTextureSgis = 33619,
                AsyncTexImageSgix = 33628,
                AsyncDrawPixelsSgix = 33629,
                AsyncReadPixelsSgix = 33630,
                FragmentLightingSgix = 33792,
                FragmentColorMaterialSgix = 33793,
                FragmentLight0Sgix = 33804,
                FragmentLight1Sgix = 33805,
                FragmentLight2Sgix = 33806,
                FragmentLight3Sgix = 33807,
                FragmentLight4Sgix = 33808,
                FragmentLight5Sgix = 33809,
                FragmentLight6Sgix = 33810,
                FragmentLight7Sgix = 33811,
                FogCoordArray = 33879,
                ColorSum = 33880,
                SecondaryColorArray = 33886,
                TextureRectangle = 34037,
                TextureCubeMap = 34067,
                ProgramPointSize = 34370,
                VertexProgramPointSize = 34370,
                VertexProgramTwoSide = 34371,
                DepthClamp = 34383,
                TextureCubeMapSeamless = 34895,
                PointSprite = 34913,
                SampleShading = 35894,
                RasterizerDiscard = 35977,
                PrimitiveRestartFixedIndex = 36201,
                FramebufferSrgb = 36281,
                SampleMask = 36433,
                PrimitiveRestart = 36765,
                DebugOutput = 37600
            };
            public enum class MaterialFace {
                Front = 1028,
                Back = 1029,
                FrontAndBack = 1032
            };
            public enum class MaterialParameter {
                Ambient = 4608,
                Diffuse = 4609,
                Specular = 4610,
                Emission = 5632,
                Shininess = 5633,
                AmbientAndDiffuse = 5634,
                ColorIndexes = 5635
            };
            public enum class PrimitiveType {
                Points = 0,
                Lines = 1,
                LineLoop = 2,
                LineStrip = 3,
                Triangles = 4,
                TriangleStrip = 5,
                TriangleFan = 6,
                Quads = 7,
                QuadsExt = 7,
                QuadStrip = 8,
                Polygon = 9,
                LinesAdjacency = 10,
                LinesAdjacencyArb = 10,
                LinesAdjacencyExt = 10,
                LineStripAdjacency = 11,
                LineStripAdjacencyArb = 11,
                LineStripAdjacencyExt = 11,
                TrianglesAdjacency = 12,
                TrianglesAdjacencyArb = 12,
                TrianglesAdjacencyExt = 12,
                TriangleStripAdjacency = 13,
                TriangleStripAdjacencyArb = 13,
                TriangleStripAdjacencyExt = 13,
                Patches = 14,
                PatchesExt = 14
            };
		}
	}
}