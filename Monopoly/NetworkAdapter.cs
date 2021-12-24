using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class NetworkAdapter
{
    public float[] pack;

    public int turn = 0;
    public int pos = 4;
    public int mon = 8;
    public int card = 12;
    public int jail = 16;
    public int own = 20;
    public int mort = 48;
    public int house = 76;
    public int select = 98;
    public int select_money = 126;

    public int[] PROPS = new int[40] { -1, 0, -1, 1, -1, 2, 3, -1, 4, 5, -1, 6, 7, 8, 9, 10, 11, -1, 12, 13, -1, 14, -1, 15, 16, 17, 18, 19, 20, 21, -1, 22, 23, -1, 24, 25, -1, 26, -1, 27 };
    public int[] HOUSES = new int[40] { -1, 0, -1, 1, -1, -1, 2, -1, 3, 4, -1, 5, -1, 6, 7, -1, 8, -1, 9, 10, -1, 11, -1, 12, 13, -1, 14, 15, -1, 16, -1, 17, 18, -1, 19, -1, -1, 20, -1, 21 };

    public NetworkAdapter()
    {
        pack = new float[127];
    }

    public void Reset()
    {
        pack = new float[127];
    }

    public float ConvertMoney(int money)
    {
        float norm = (float)money / 4000.0f;
        float clamp = Math.Clamp(norm, 0.0f, 1.0f);

        return clamp;
    }

    public float ConvertMoneyValue(float value)
    {
        return value * 4000.0f;
    }

    public float ConvertHouseValue(float value)
    {
        if (value <= 0.5f)
        {
            value = 0.0f;
        }

        return value * 15.0f;
    }

    public float ConvertPosition(int position)
    {
        float norm = (float)position / 39.0f;
        float clamp = Math.Clamp(norm, 0.0f, 1.0f);

        return clamp;
    }

    public float ConvertCard(int cards)
    {
        float clamp = Math.Clamp(card, 0.0f, 1.0f);
        return clamp;
    }

    public float ConvertHouse(int houses)
    {
        float norm = (float)houses / 5.0f;
        float clamp = Math.Clamp(norm, 0.0f, 1.0f);

        return clamp;
    }

    public void SetTurn(int index)
    {
        for (int i = 0; i < 4; i++)
        {
            pack[i] = 0.0f;
        }

        pack[index] = 1.0f;
    }

    public void SetSelection(int index)
    {
        for (int i = select; i < select + 29; i++)
        {
            pack[i] = 0.0f;
        }

        pack[select + PROPS[index]] = 1.0f;
    }

    public void SetSelectionState(int index, int state)
    {
        pack[select + PROPS[index]] = state;
    }

    public void SetMoneyContext(int state)
    {
        pack[select_money] = state;
    }

    public void ClearSelectionState()
    {
        for (int i = select; i < select + 29; i++)
        {
            pack[i] = 0.0f;
        }
    }

    public void SetPosition(int index, int position)
    {
        pack[pos + index] = ConvertPosition(position);
    }

    public void SetMoney(int index, int money)
    {
        pack[mon + index] = ConvertMoney(money);
    }

    public void SetCard(int index, int cards)
    {
        pack[card + index] = ConvertCard(cards);
    }

    public void SetJail(int index, int state)
    {
        pack[jail + index] = state;
    }

    public void SetOwner(int property, int state)
    {
        float convert = (state + 1) / 4.0f;

        pack[own + PROPS[property]] = convert;
    }

    public void SetMortgage(int property, int state)
    {
        pack[mort + PROPS[property]] = state;
    }

    public void SetHouse(int property, int houses)
    {
        pack[house + HOUSES[property]] = ConvertHouse(houses);
    }
}
