using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using System.Diagnostics;

namespace ProjectDawn.Navigation
{
    public unsafe struct GizmosCommandBuffer : IDisposable
    {
        NativeList<byte> m_Commands;

        public GizmosCommandBuffer(Allocator allocator)
        {
            m_Commands = new NativeList<byte>(1024 * 8, allocator);
        }

        /// <summary>An extension of EntityCommandBuffer that allows concurrent (deterministic) command buffer recording.</summary>
        /// <returns>The <see cref="ParallelWriter"/> that can be used to record commands in parallel.</returns>
        public ParallelWriter AsParallelWriter()
        {
            return new ParallelWriter(m_Commands);
        }

        public void Execute()
        {
#if UNITY_EDITOR
            var reader = AsReader();
            while (!reader.IsEmpty)
            {
                var command = reader.Read<CommandType>();
                switch (command)
                {
                    case CommandType.SolidArc:
                        UnityEditor.Handles.color = reader.Read<Color>();
                        UnityEditor.Handles.zTest = UnityEngine.Rendering.CompareFunction.Disabled;
                        UnityEditor.Handles.DrawSolidArc(reader.Read<Vector3>(), reader.Read<Vector3>(), reader.Read<Vector3>(), reader.Read<float>(), reader.Read<float>());
                        break;
                    case CommandType.WireArc:
                        UnityEditor.Handles.color = reader.Read<Color>();
                        UnityEditor.Handles.zTest = UnityEngine.Rendering.CompareFunction.Disabled;
                        UnityEditor.Handles.DrawWireArc(reader.Read<Vector3>(), reader.Read<Vector3>(), reader.Read<Vector3>(), reader.Read<float>(), reader.Read<float>());
                        break;
                    case CommandType.SolidDisc:
                        UnityEditor.Handles.color = reader.Read<Color>();
                        UnityEditor.Handles.zTest = UnityEngine.Rendering.CompareFunction.Disabled;
                        UnityEditor.Handles.DrawSolidDisc(reader.Read<Vector3>(), reader.Read<Vector3>(), reader.Read<float>());
                        break;
                    case CommandType.Line:
                        UnityEditor.Handles.color = reader.Read<Color>();
                        UnityEditor.Handles.zTest = UnityEngine.Rendering.CompareFunction.Disabled;
                        UnityEditor.Handles.DrawLine(reader.Read<Vector3>(), reader.Read<Vector3>());
                        break;
                    case CommandType.Arrow:
                        UnityEditor.Handles.color = reader.Read<Color>();
                        UnityEditor.Handles.zTest = UnityEngine.Rendering.CompareFunction.Disabled;
                        UnityEditor.Handles.ArrowHandleCap(0, reader.Read<Vector3>(), quaternion.LookRotation(reader.Read<Vector3>(), new float3(0, 1, 0)), reader.Read<float>(), EventType.Repaint); // TODO
                        break;
                    case CommandType.WireBox:
                        UnityEditor.Handles.color = reader.Read<Color>();
                        UnityEditor.Handles.zTest = UnityEngine.Rendering.CompareFunction.Disabled;
                        UnityEditor.Handles.DrawWireCube(reader.Read<Vector3>(), reader.Read<Vector3>());
                        break;
                    case CommandType.AAConvexPolygon:
                        {
                            UnityEditor.Handles.color = reader.Read<Color>();
                            UnityEditor.Handles.zTest = reader.Read<bool>() ? UnityEngine.Rendering.CompareFunction.LessEqual : UnityEngine.Rendering.CompareFunction.Disabled;
                            Vector3[] vertices = new Vector3[reader.Read<int>()];
                            for (int i = 0; i < vertices.Length; ++i)
                                vertices[i] = reader.Read<Vector3>();
                            UnityEditor.Handles.DrawAAConvexPolygon(vertices);
                            break;
                        }
                    case CommandType.Quad:
                        {
                            UnityEditor.Handles.color = reader.Read<Color>();
                            UnityEditor.Handles.zTest = reader.Read<bool>() ? UnityEngine.Rendering.CompareFunction.LessEqual : UnityEngine.Rendering.CompareFunction.Disabled;
                            Vector3[] vertices = new Vector3[]
                            {
                                reader.Read<Vector3>(),
                                reader.Read<Vector3>(),
                                reader.Read<Vector3>(),
                                reader.Read<Vector3>()
                            };
                            UnityEditor.Handles.DrawAAConvexPolygon(vertices);
                            break;
                        }
                    default:
                        throw new InvalidOperationException($"Unknown draw command {command}.");
                }

                reader.ReadEnd(command);
            }
#endif
        }

        Reader AsReader()
        {
            return new Reader(m_Commands);
        }


        public void Clear()
        {
            m_Commands.Clear();
        }

        public void Dispose()
        {
            m_Commands.Dispose();
        }

        struct Reader
        {
            NativeList<byte> m_Commands;
            int m_Index;

            public bool IsEmpty => m_Index == m_Commands.Length;

            public Reader(NativeList<byte> commands)
            {
                m_Commands = commands;
                m_Index = 0;
            }

            public T Read<T>() where T : unmanaged
            {
                int sizeOf = sizeof(T);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (m_Index + sizeOf > m_Commands.Length)
                    throw new InvalidOperationException($"Can not read value type {typeof(T).Name} at index {m_Index} as it exceeds buffer size {m_Commands.Length}. This can happen if read and write where not in sync.");
#endif
                var ptr = (byte*) m_Commands.GetUnsafePtr() + m_Index;
                m_Index += sizeOf;
                return *((T*) ptr);
            }

            [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
            public void ReadEnd(CommandType command)
            {
                if (Read<CommandType>() != CommandType.None)
                    throw new InvalidOperationException($"Draw command {command} failed to read end.");
            }
        }

        public struct ParallelWriter
        {
            NativeList<byte>.ParallelWriter m_Commands;

            public ParallelWriter(NativeList<byte> commands)
            {
                m_Commands = commands.AsParallelWriter();
            }

            public void DrawSolidArc(float3 center, float3 normal, float3 from, float angle, float radius, Color color)
            {
                Write(CommandType.SolidArc);
                Write(color);
                Write(center);
                Write(normal);
                Write(from);
                Write(angle);
                Write(radius);
                WriteEnd();
            }

            public void DrawWireArc(float3 center, float3 normal, float3 from, float angle, float radius, Color color)
            {
                Write(CommandType.WireArc);
                Write(color);
                Write(center);
                Write(normal);
                Write(from);
                Write(angle);
                Write(radius);
                WriteEnd();
            }

            public void DrawSolidDisc(float3 center, float3 normal, float radius, Color color)
            {
                Write(CommandType.SolidDisc);
                Write(color);
                Write(center);
                Write(normal);
                Write(radius);
                WriteEnd();
            }

            public void DrawWireBox(float3 position, float3 size, Color color)
            {
                Write(CommandType.WireBox);
                Write(color);
                Write(position);
                Write(size);
                WriteEnd();
            }

            public void DrawLine(float3 from, float3 to, Color color)
            {
                Write(CommandType.Line);
                Write(color);
                Write(from);
                Write(to);
                WriteEnd();
            }

            public void DrawArrow(float3 origin, float3 direction, float size, Color color)
            {
                Write(CommandType.Arrow);
                Write(color);
                Write(origin);
                Write(direction);
                Write(size);
                WriteEnd();
            }

            public void DrawAAConvexPolygon(NativeArray<float3> vertices, Color color, bool zTest = false)
            {
                Write(CommandType.AAConvexPolygon);
                Write(color);
                Write(zTest);
                Write(vertices.Length);
                m_Commands.AddRangeNoResize(vertices.GetUnsafePtr(), sizeof(float3) * vertices.Length);
                WriteEnd();
            }

            public void DrawQuad(float3 a, float3 b, float3 c, float3 d, Color color, bool zTest = false)
            {
                Write(CommandType.Quad);
                Write(color);
                Write(zTest);
                Write(a);
                Write(b);
                Write(c);
                Write(d);
                WriteEnd();
            }

            void Write<T>(T value) where T : unmanaged
            {
                m_Commands.AddRangeNoResize(&value, sizeof(T));
            }

            [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
            void WriteEnd() => Write(CommandType.None);
        }

        enum CommandType
        {
            None,
            SolidArc,
            WireArc,
            Line,
            Arrow,
            SolidDisc,
            WireBox,
            AAConvexPolygon,
            Quad,
        }
    }
}
