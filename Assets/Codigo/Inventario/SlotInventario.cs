using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SlotInventario
{
    private List<IItemBase> itemsSlot =  new List<IItemBase>();
    private int maxItems;

    public int AgregarItem(IItemBase item)
    {
        if(itemsSlot.Count == 0) //Primer objeto del slot
        {
            switch (item.TipoObjeto)
            {
                case TipoObjeto.Peque:
                    maxItems = 100;
                    break;
                case TipoObjeto.Mediano:
                    maxItems = 30;
                    break;
                case TipoObjeto.Grande:
                    maxItems = 5;
                    break;
                case TipoObjeto.NoStack:
                    maxItems = 1;
                    break;
            }
            itemsSlot.Add(item);
            return 1;
        }
        else if(itemsSlot.Count >= maxItems) //No entra el objeto en el slot
        {
            return -1;
        }
        else if(itemsSlot.Count < maxItems && item.Nombre == itemsSlot[0].Nombre) //Entra el objeto en el slot
        {
            itemsSlot.Add(item);
            return itemsSlot.Count;
        }
        else
        {
            return -1;
        }
    }

    public IItemBase ObtenerItem()
    {
        if(itemsSlot.Count > 0)
        {
            return itemsSlot[0];
        }
        else
        {
            return null;
        }
    }

    public int QuitarItem()
    {
        if (itemsSlot.Count > 0)
        {
            itemsSlot.RemoveAt(0);
            return itemsSlot.Count;
        }
        else
        {
            return -1;
        }
    }

    public int ObtenetNumeroItems()
    {
        return itemsSlot.Count;
    }
}
