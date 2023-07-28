using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidFieldManager : MonoBehaviour {

    public static List<AsteroidField> AsteroidFields;

    public static void Reset() {
        AsteroidFields = new List<AsteroidField>();
    }

    public static AsteroidField GetRandomAsteroidField() {
        return AsteroidFields[Random.Range(0, AsteroidFields.Count)];
    }

    public static AsteroidField GetClosestAsteroidField(Vector2 location) {
        AsteroidField currentAsteroidField = null;
        float currentDistance = float.MaxValue;
        foreach (AsteroidField field in AsteroidFields) {
            float distanceToField = Vector2.Distance(location, field.transform.position);
            if (currentDistance > distanceToField) {
                currentAsteroidField = field;
                currentDistance = distanceToField;
            }
        }

        return currentAsteroidField;
    }

    public static List<AsteroidField> GetAsteroidFieldsWithRoomForMines() {
        List<AsteroidField> fields = new List<AsteroidField>();
        foreach (AsteroidField field in AsteroidFields) {
            if (field.RoomForMine()) fields.Add(field);
        }

        return fields;
    }

    public static List<AsteroidField> GetAsteroidFieldsSortedByDistanceToLocation(Vector2 location) {
        AsteroidFields.Sort(delegate (AsteroidField first, AsteroidField second) {
            return (int)(Vector2.Distance(first.transform.position, location) - Vector2.Distance(second.transform.position, location));
        });
        return AsteroidFields;
    }

    public static int HowManyFreeMineSpots() {
        int toReturn = 0;
        foreach (AsteroidField field in AsteroidFields) {
            if (field.RoomForMine()) {
                toReturn++;
            }
        }

        return toReturn;
    }
}
