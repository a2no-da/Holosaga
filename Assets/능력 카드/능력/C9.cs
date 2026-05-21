using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "카드 / 주스 파티")]
public class C9 : AbilityFunction
{
    public float damage;
    public GameObject platePrefab;
    public GameObject potionPrefab;
    private LevelManager levelManager;

    public override void FunctionCard(Tower tower)
    {
        if (tower.isEx) { return; }
    }

    public override void FunctionDesCard(Tower tower)
    {
    }

    public override void FunctionPulsACard(Unit Unit)
    {
        if (levelManager == null)
        {
            levelManager = LevelManager.Instance;
        }
        levelManager.OccupiedPoints.Clear();

        if (Random.Range(0f, 1f) <= 1f)
        {
            List<Point> keys = new List<Point>(levelManager.Tiles.Keys.Except(levelManager.OccupiedPoints));

            if (keys.Count > 0)
            {
                Point randomKey = keys[Random.Range(0, keys.Count)];
                levelManager.OccupiedPoints.Add(randomKey);
                keys.Remove(randomKey);

                CreatePotion(randomKey, "damaging", Unit);
            }
        }
    }

    private void CreatePotion(Point key, string potionAnimation, Unit Unit)
    {
        Tile tile = levelManager.Tiles[key];
        GameObject potionObject = Instantiate(potionPrefab, Unit.transform.position, Quaternion.identity);
        Potion potion = potionObject.GetComponent<Potion>();

        Vector3[] additionalPositions = new Vector3[]
   {
        new Vector3(6.599999f, 0.6600003f, 0), 
        new Vector3(8.249999f, 0.6600003f, 0), 
        new Vector3(6.599999f, -1.36f, 0),
        new Vector3(8.249999f, -1.36f, 0), 
        new Vector3(6.599999f, -3.38f, 0), 
        new Vector3(8.249999f, -3.38f, 0), 
        new Vector3(6.599999f, -5.4f, 0), 
        new Vector3(8.249999f, -5.4f, 0) 
   };

        List<Vector3> possiblePositions = new List<Vector3> { tile.transform.position };

        if (Random.Range(0f, 1f) <= 0.25f)
        {
            possiblePositions.Add(additionalPositions[Random.Range(0, additionalPositions.Length)]);
        }

        Vector3 randomPosition = possiblePositions[Random.Range(0, possiblePositions.Count)];
        float duration = 1.6667f;
        potion.SetAnimation(potionAnimation);
        potion.MoveTo(randomPosition, duration);

        potion.OnAnimationEnd += () =>
        {
            GameObject plateObject = Instantiate(platePrefab, randomPosition, Quaternion.identity);
            AcPlate plate = plateObject.GetComponent<AcPlate>();
            if (plateObject != null)
            {
                plate.Initialize(damage, Unit);
            }
            plate.SetAnimation("abilityCard");
        };
    }

    public override void FunctionWorldCard()
    {
    }
}
