using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static class housing all the methods for calculating customer satisfaction.
/// All methods in this class should NOT mutate the customer.
/// </summary>
public static class CustomerSatisfaction
{
    public static float calculateCustomerSatisfaction(int totalItemsOrdered, float waitBetweenOrders, float maxWaitTimeSecondsForOrder, float timeWaited, float bonus)
    {
        float avgTimeSpentOnEachOrder;
        float worstTimeAvgTimeSpentOnEachOrder;
        float satisfaction;

        // worst time is as if we waited max time for each order and max
        // time in between each orders
        float worstTime;

        if (totalItemsOrdered != 0)
        {
            worstTime = maxWaitTimeSecondsForOrder * totalItemsOrdered + ((totalItemsOrdered - 1) * waitBetweenOrders);
            avgTimeSpentOnEachOrder = (timeWaited / totalItemsOrdered);
            worstTimeAvgTimeSpentOnEachOrder = worstTime / totalItemsOrdered;

            // we know that the average time waited will be between 0 and worstTimeAvgTimeSpentOnEachOrder
            satisfaction = Mathf.InverseLerp(worstTimeAvgTimeSpentOnEachOrder, 0, avgTimeSpentOnEachOrder);
        }
        else
        {
            worstTime = maxWaitTimeSecondsForOrder;
            avgTimeSpentOnEachOrder = timeWaited;
            satisfaction = Mathf.InverseLerp(worstTime, 0, avgTimeSpentOnEachOrder);
        }

        // lerp it again with some buffer, to account for the fact that there must be some amount of wait time
        return Mathf.Lerp(0, 1, Mathf.Min(satisfaction + 0.3f + bonus, 1f));
    }

    public static bool isCustomerSatisfiedWithSeating(Chair chair, bool isSocial)
    {
        Table table = chair.getTable();
        if (table.GetIndependentCustomersCount() > 0 && table.GetSocialCustomersCount() > 0)
        {
            // We have at least one independent customer and one social customer at this table.
            // Conflicting interests makes this customer not satisfied.
            return false;
        }

        int totalCustomersAtTable = table.GetSocialCustomersCount() + table.GetIndependentCustomersCount();
        if (totalCustomersAtTable > 1 && isSocial)
        {
            return true;
        }
        if (totalCustomersAtTable == 1 && !isSocial)
        {
            return true;
        }
        return false;
    }

}
