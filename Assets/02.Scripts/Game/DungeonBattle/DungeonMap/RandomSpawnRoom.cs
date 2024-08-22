using System;
using System.Collections.Generic;

public class RandomSpawnRoom
{
  public static List<T> GetRandomValues<T>(List<T> sourceList, int count)
  {
    if (sourceList == null || sourceList.Count < count)
    {
      throw new ArgumentException("Source list is null or does not have enough elements.");
    }

    List<T> result = new List<T>();
    Random random = new Random();
    HashSet<int> selectedIndices = new HashSet<int>();

    while (result.Count < count)
    {
      int index = random.Next(sourceList.Count);

      // Check if the index has already been selected
      if (!selectedIndices.Contains(index))
      {
        selectedIndices.Add(index);
        result.Add(sourceList[index]);
      }
    }

    return result;
  }
}