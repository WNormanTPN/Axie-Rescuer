using System;
using Unity.Entities;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Burst;
using UnityEngine;
using static Unity.Entities.SystemAPI;

namespace ProjectDawn.Navigation
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public unsafe partial struct GizmosSystem : ISystem
    {
        UnsafeList<GizmosCommandBuffer>* m_CommandBuffers;
        NativeQueue<int> m_FreeCommandBufferHandles;
        NativeList<int> m_ActiveCommandBufferHandle;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            m_CommandBuffers = UnsafeList<GizmosCommandBuffer>.Create(1, Allocator.Persistent);
            m_FreeCommandBufferHandles = new NativeQueue<int>(Allocator.Persistent);
            m_ActiveCommandBufferHandle = new NativeList<int>(Allocator.Persistent);

            state.EntityManager.AddComponentData(state.SystemHandle, new Singleton
            {
                m_CommandBuffers = m_CommandBuffers,
                m_ActiveCommandBufferHandle = m_ActiveCommandBufferHandle,
                m_FreeCommandBufferHandles = m_FreeCommandBufferHandles,
            });
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            for (int handle = 0; handle < m_CommandBuffers->Length; ++handle)
                m_CommandBuffers->ElementAt(handle).Dispose();
            UnsafeList<GizmosCommandBuffer>.Destroy(m_CommandBuffers);
            m_FreeCommandBufferHandles.Dispose();
            m_ActiveCommandBufferHandle.Dispose();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency.Complete();
            GetSingletonRW<Singleton>().ValueRW.Clear();
        }

        public struct Singleton : IComponentData
        {
            [NativeDisableUnsafePtrRestriction]
            internal UnsafeList<GizmosCommandBuffer>* m_CommandBuffers;
            internal NativeQueue<int> m_FreeCommandBufferHandles;
            internal NativeList<int> m_ActiveCommandBufferHandle;

            public GizmosCommandBuffer CreateCommandBuffer()
            {
                if (m_FreeCommandBufferHandles.TryDequeue(out int handle))
                {
                    m_ActiveCommandBufferHandle.Add(handle);
                    return m_CommandBuffers->ElementAt(handle);
                }
                else
                {
                    handle = m_CommandBuffers->Length;
                    var commandBuffer = new GizmosCommandBuffer(Allocator.Persistent);
                    m_CommandBuffers->Add(commandBuffer);
                    m_ActiveCommandBufferHandle.Add(handle);
                    return commandBuffer;
                }
            }

            public void Clear()
            {
                foreach (var handle in m_ActiveCommandBufferHandle)
                {
                    m_CommandBuffers->ElementAt(handle).Clear();
                    m_FreeCommandBufferHandles.Enqueue(handle);
                }
                m_ActiveCommandBufferHandle.Clear();
            }

            public void ExecuteCommandBuffers()
            {
                foreach (var handle in m_ActiveCommandBufferHandle)
                {
                    ref var cmd = ref m_CommandBuffers->ElementAt(handle);
                    cmd.Execute();
                }
            }
        }
    }
}
