using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Static class stats contains methods required for saving
 * the statistics of how well a player did on a given
 * day. This information will be reported back to the
 * player.
 */

public struct CustomerStats
{
    public readonly float customerSatisfactionScore;
    public readonly int itemsOrdered;

    public CustomerStats(float score, int numItems)
    {
        customerSatisfactionScore = score;
        itemsOrdered = numItems;
    }
}

public struct DayStats
{
    int customersServed;
    float runningTotalSatisfactionScore;
    int runningTotalItemsOrdered;
    float moneyMade;
    float lostMoney;
    int itemsTrashed;

    public DayStats(int a, float b, int c, float d, float e, int f)
    {
        customersServed = a;
        runningTotalSatisfactionScore = b;
        runningTotalItemsOrdered = c;
        moneyMade = d;
        lostMoney = e;
        itemsTrashed = f;
    }

    public float getAverageSatisfactionScore()
    {
        return (float) System.Math.Round(runningTotalSatisfactionScore / customersServed, 2);
    }

    public double getAverageItemsOrderedPerCustomer()
    {
        return System.Math.Round((double)(runningTotalItemsOrdered / customersServed), 1);
    }

    public int getMoneyMade()
    {
        return Mathf.RoundToInt(moneyMade);
    }

    public void setMoneyMade(float money)
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

    public int getItemsServed()
    {
        return runningTotalItemsOrdered;
    }

    public int getLostMoney()
    {
        return Mathf.RoundToInt(lostMoney);
    }

    public void pushCustomerStats(CustomerStats customerStats)
    {
        runningTotalSatisfactionScore += customerStats.customerSatisfactionScore;
        runningTotalItemsOrdered += customerStats.itemsOrdered;
        customersServed++;
    }

    public void pushTrashStats(int itemsTrashed, float lostMoney)
    {
        this.itemsTrashed = itemsTrashed;
        if (float.IsNaN(lostMoney))
        {
            this.lostMoney = lostMoney;
        } else
        {
            this.lostMoney += lostMoney;
        }
    }
}

public static class Stats
{
    private static DayStats yesterdayStats;
    private static DayStats todayStats = new DayStats(0, 1f, 0, 0, 0, 0);
    private static float currentRealtimeAvgCustomerSatisfaction = 1f;

    public static void clearStatsForDay()
    {
        yesterdayStats = todayStats;
        todayStats = new DayStats(0, 1f, 0, 0, 0, 0);
    }

    public static void addCustomerStats(CustomerStats statsObj)
    {
        todayStats.pushCustomerStats(statsObj);
    }

    public static void addTrashStats(int itemsTrashed, float lostMoney)
    {
        todayStats.pushTrashStats(itemsTrashed, lostMoney);
    }

    private static Dictionary<string, string> queryTodayStatsDict()
    {
        Dictionary<string, string> statsDictionary = new Dictionary<string, string>();

        statsDictionary.Add("MoneyEarned", todayStats.getMoneyMade().ToString());
        statsDictionary.Add("CustomersServed", todayStats.getCustomersServed().ToString());
        statsDictionary.Add("AvgCustomerSatisfaction",
            System.Math.Round(todayStats.getAverageSatisfactionScore() * 5, 1).ToString() + "/5");

        return statsDictionary;
    }

    public static int queryTodayMoneyMade()
    {
        int money = todayStats.getMoneyMade();
        return float.IsNaN(money) ? 0 : money;
    }

    public static int queryTodayCustomersServed()
    {
        int customers = todayStats.getCustomersServed();
        return float.IsNaN(customers) ? 0 : customers;
    }

    public static float queryTodayCustomerSatisfaction()
    {
        float score = todayStats.getAverageSatisfactionScore();
        return float.IsNaN(score) ? 0 : score;
    }

    public static int queryTodayItemsServed()
    {
        int items = todayStats.getItemsServed();
        return float.IsNaN(items) ? 0 : items;
    }

    public static int queryLostMoney()
    {
        int money = todayStats.getLostMoney();
        return float.IsNaN(money) ? 0 : money;
    }

    public static Dictionary<string, string> yesterdayTodayDiffStats()
    {
        Dictionary<string, string> statsDictionary = new Dictionary<string, string>();

        statsDictionary.Add("MoneyEarned", (todayStats.getMoneyMade() - yesterdayStats.getMoneyMade()).ToString());
        statsDictionary.Add("CustomersServed", (todayStats.getCustomersServed() - yesterdayStats.getMoneyMade()).ToString());
        statsDictionary.Add("AvgCustomerSatisfaction",
            System.Math.Round((todayStats.getAverageSatisfactionScore() - yesterdayStats.getAverageSatisfactionScore()) * 100).ToString() + "%");

        return statsDictionary;
    }

    public static float queryRealTimeAvgCustomerSatisfaction()
    {
        return currentRealtimeAvgCustomerSatisfaction;
    }

    public static void pushRealTimeAvgCustomerSatisfaction(float value)
    {
        currentRealtimeAvgCustomerSatisfaction = value;
    }
}

