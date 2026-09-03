using UnityEngine;

public class BlockGenerator : MonoBehaviour
{
    [Header("Walls")]
    public BoxCollider2D leftWall;
    public BoxCollider2D rightWall;
    public BoxCollider2D topWall;

    [Header("Blocks")]
    public GameObject[] blockPrefabs;
    public int minRows = 3;
    public int maxRows = 6;
    public int minColumns = 5;
    public int maxColumns = 10;

    [Header("Spacing")]
    public float horizontalSpacing = 0.15f;
    public float verticalSpacing = 0.15f;
    public float topMargin = 0.5f;
    public float sideMargin = 0.25f;

    //----------------------------------------------------------------------------------------

    public int GenerateBlocks(int level)
    {
        if (blockPrefabs == null || blockPrefabs.Length == 0) return 0;

        // Alternates level progression between rows and columns:
        // even levels increase rows, while odd levels increase columns, up to their maximum values.
        var rows = Mathf.Min(minRows + (level / 2), maxRows);
        var columns = Mathf.Min(minColumns + ((level - 1) / 2), maxColumns);

        // Calculates the area available between the walls.
        var minX = leftWall.bounds.max.x + sideMargin;
        var maxX = rightWall.bounds.min.x - sideMargin;
        var maxY = topWall.bounds.min.y - topMargin;

        var availableWidth = maxX - minX;
        // Calculates the maximum block width based on the available space.
        var totalSpacing = horizontalSpacing * (columns - 1);
        var blockWidth = (availableWidth - totalSpacing) / columns;

        var generatedBlocks = 0;
        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                var prefab = blockPrefabs[Random.Range(0, blockPrefabs.Length)];
                var block = Instantiate(prefab, transform);
                generatedBlocks++;

                var spriteRenderer = block.GetComponent<SpriteRenderer>();

                // Scales the block to fit horizontally between the walls.
                var originalWidth = spriteRenderer.sprite.bounds.size.x * block.transform.localScale.x;
                var scaleFactor = blockWidth / originalWidth;

                block.transform.localScale *= scaleFactor;
                var blockHeight = spriteRenderer.sprite.bounds.size.y * block.transform.localScale.y;

                // Positions the block.
                var x = minX + blockWidth / 2f + column * (blockWidth + horizontalSpacing);
                var y = maxY - blockHeight / 2f - row * (blockHeight + verticalSpacing);
                block.transform.position = new Vector3(x, y, transform.position.z);
            }
        }

        return generatedBlocks;
    }
}