using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Static class stats contains methods required for saving
 * the statistics of how well a player did on a given
 * day. This information will be reported back to the
 * player.
 */

public readonly struct CustomerStats
{
    public readonly float customerSatisfactionScore;
    public readonly int itemsOrdered;
}

public struct DayStats
{
    int customersServed;
    float runningTotalSatisfactionScore;
    float runningTotalItemsOrdered;
    int moneyMade;

    public DayStats(int a, float b, int c, int d)
    {
        customersServed = a;
        runningTotalSatisfactionScore = b;
        runningTotalItemsOrdered = c;
        moneyMade = d;
    }

    public double getAverageSatisfactionScore()
    {
        return System.Math.Round(runningTotalSatisfactionScore / customersServed, 2);
    }

    public double getAverageItemsOrderedPerCustomer()
    {
        return System.Math.Round(runningTotalItemsOrdered / customersServed, 1);
    }

    public int getMoneyMade()
    {
        return moneyMade;
    }

    public void setMoneyMade(int money)
    {
        moneyMade = money;
    }

    public int getCustomersServed()
    {
        return customersServed;
    }

    public void setCustomersServed(int numCustomers)
    {
        customersServed = numCustomers;
    }

    public void pushCustomerStats(CustomerStats customerStats)
    {
        runningTotalSatisfactionScore += customerStats.customerSatisfactionScore;
        runningTotalItemsOrdered += customerStats.itemsOrdered;
    }
}

public static class Stats
{
    private static DayStats yesterdayStats;
    private static DayStats todayStats;


    public static void clearStatsForDay()
    {
        yesterdayStats = todayStats;
        todayStats = new DayStats { };
    }

    public static void addCustomerStats(CustomerStats statsObj)
    {
        todayStats.pushCustomerStats(statsObj);
    }

    public static Dictionary<string, string> queryTodayStats()
    {
        Dictionary<string, string> statsDictionary = new Dictionary<string, string>();

        statsDictionary.Add("Money earned", todayStats.getMoneyMade().ToString());
        statsDictionary.Add("Customers served", todayStats.getCustomersServed().ToString());
        statsDictionary.Add("Average customer satisfaction",
            System.Math.Round(todayStats.getAverageSatisfactionScore() * 5, 1).ToString() + "/5");

        return statsDictionary;
    }

    public static Dictionary<string, string> yesterdayTodayDiffStats()
    {
        Dictionary<string, string> statsDictionary = new Dictionary<string, string>();

        statsDictionary.Add("Money earned", (todayStats.getMoneyMade() - yesterdayStats.getMoneyMade()).ToString());
        statsDictionary.Add("Customers served", (todayStats.getCustomersServed() - yesterdayStats.getMoneyMade()).ToString());
        statsDictionary.Add("Average customer satisfaction",
            System.Math.Round((todayStats.getAverageSatisfactionScore() - yesterdayStats.getAverageSatisfactionScore()) * 100).ToString() + "%");

        return statsDictionary;
    }
}

