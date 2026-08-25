using System.Collections.Generic;
using UnityEngine;

namespace ZombieElimination
{
    public class ProceduralGenerator : MonoBehaviour
    {
        [Header("Block Prefabs & Pools")]
        [Tooltip("Prefabs of blocks to pool and spawn.")]
        public List<Block> prefabBlocks = new List<Block>();

        private List<AdvancedObjectPool<Block>> blockPools;

        [Header("Block Management")]
        [Tooltip("Blocks currently in the level (ordered: behind -> current -> ahead).")]
        public List<Block> activeBlocks = new List<Block>();

        [Tooltip("How many blocks should always be ahead of the player.")]
        public int blocksAhead = 2;

        [Tooltip("How many blocks should remain behind the player.")]
        public int blocksBehind = 2;

        // Tracks the current block the progression refers to
        private Block currentBlock;
        private int currentBlockIndex = -1;

        // For chaining and indexing new blocks
        private Block lastBlock;
        private int nextBlockIndex = 0;

        private void Awake()
        {
            // Initialize object pools for each block prefab
            blockPools = new List<AdvancedObjectPool<Block>>();
            foreach (var blockPrefab in prefabBlocks)
            {
                var poolParent = new GameObject($"{blockPrefab.name}_Pool");
                poolParent.transform.SetParent(transform);
                blockPools.Add(new AdvancedObjectPool<Block>(blockPrefab, 10, parent: poolParent.transform));
            }
        }

        private void Start()
        {
            InitializeFromSceneBlocks();
            EnsureBlocksWindow();

            // Subscribe to external block triggered event (make sure this exists)
            EventManager.Instance.OnBlockTriggered += OnBlockTriggered;
        }

        /// <summary>
        /// Find and initialize active blocks from manually preplaced blocks in the hierarchy.
        /// </summary>
        private void InitializeFromSceneBlocks()
        {
            activeBlocks.Clear();

            Block[] foundBlocks = GetComponentsInChildren<Block>(false);

            if (foundBlocks.Length == 0)
            {
                Debug.LogWarning("No preplaced blocks found! Add blocks manually in the scene.");
                return;
            }

            // Sort blocks by their Z position (assuming blocks arranged along Z axis)
            System.Array.Sort(foundBlocks, (a, b) => a.transform.position.z.CompareTo(b.transform.position.z));

            for (int i = 0; i < foundBlocks.Length; i++)
            {
                foundBlocks[i].blockIndex = i;
            }

            activeBlocks.AddRange(foundBlocks);

            currentBlockIndex = 0;
            currentBlock = activeBlocks[0];
            lastBlock = activeBlocks[activeBlocks.Count - 1];
            nextBlockIndex = activeBlocks.Count;
        }

        /// <summary>
        /// Ensure the sliding window is valid by spawning enough blocks ahead.
        /// </summary>
        private void EnsureBlocksWindow()
        {
            int blocksAheadPresent = activeBlocks.Count - 1 - currentBlockIndex;
            while (blocksAheadPresent < blocksAhead)
            {
                Block newBlock = SpawnNewBlock();
                activeBlocks.Add(newBlock);
                blocksAheadPresent++;
            }
            // At start, do not remove blocks behind; pruning happens when progression occurs.
        }

        /// <summary>
        /// Called when a block is triggered to advance current block and manage window.
        /// </summary>
        /// <param name="triggeredBlock">The block which was triggered.</param>
        public void OnBlockTriggered(Block triggeredBlock)
        {
            int triggeredIndex = activeBlocks.IndexOf(triggeredBlock);
            if (triggeredIndex == -1) return;

            // Advance currentBlock to the next block if exists, else stay
            if (triggeredIndex + 1 < activeBlocks.Count)
            {
                currentBlock = activeBlocks[triggeredIndex + 1];
                currentBlockIndex = triggeredIndex + 1;
            }
            else
            {
                currentBlock = activeBlocks[triggeredIndex];
                currentBlockIndex = triggeredIndex;
            }

            // Spawn blocks ahead as needed
            int blocksAheadPresent = activeBlocks.Count - 1 - currentBlockIndex;
            while (blocksAheadPresent < blocksAhead)
            {
                Block newBlock = SpawnNewBlock();
                activeBlocks.Add(newBlock);
                blocksAheadPresent++;
            }

            // Remove blocks beyond blocksBehind behind currentBlock, only if there are enough to remove
            while (currentBlockIndex > blocksBehind)
            {
                RemoveBlock(activeBlocks[0]);
                activeBlocks.RemoveAt(0);
                currentBlockIndex--;
            }
        }

        /// <summary>
        /// Spawn a new block, chain it from the last block, and activate it.
        /// </summary>
        /// <returns>The spawned block instance.</returns>
        private Block SpawnNewBlock()
        {
            int poolIdx = Random.Range(0, blockPools.Count);
            AdvancedObjectPool<Block> pool = blockPools[poolIdx];
            Block block = pool.Get();

            block.blockIndex = nextBlockIndex++;
            block.ResetBlockState();

            if (lastBlock == null)
            {
                block.transform.position = Vector3.zero;
                block.transform.rotation = Quaternion.identity;
            }
            else
            {
                Transform lastEnd = lastBlock.EndPoint;
                Transform thisPivot = block.PivotPoint;

                if (lastEnd == null || thisPivot == null)
                {
                    Debug.LogWarning("PivotPoint or EndPoint missing in block prefab!");
                }
                else
                {
                    Vector3 offset = lastEnd.position - thisPivot.position;
                    block.transform.position += offset;
                    block.transform.rotation = lastBlock.transform.rotation;
                }
            }

            lastBlock = block;
            block.gameObject.SetActive(true);


            return block;
        }

        /// <summary>
        /// Safely removes a block from the scene and returns it to the pool.
        /// </summary>
        private void RemoveBlock(Block block)
        {
            if (block == null) return;

            block.ResetBlockState();
            block.gameObject.SetActive(false);

            for (int i = 0; i < prefabBlocks.Count; i++)
            {
                if (block.blockID == prefabBlocks[i].blockID)
                {
                    blockPools[i].Release(block);
                    return;
                }
            }

            // Fallback: destroy block if pool not found (should rarely happen)
            Destroy(block.gameObject);
        }
    }
}
