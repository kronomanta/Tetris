public static class Extensions {

    public static int FirstIndexOfDifferent<T>(this T[] list, T element)
    {
        for (int i = 0; i < list.Length; i++)
        {
            if (!Equals(list[i], element)) return i;
        }

        return -1;
    }

    public static int LastIndexOfDifferent<T>(this T[] list, T element)
    {
        int i = list.Length-1;
        for(; i> -1; i--)
        {
            if (!Equals(list[i], element)) return i;
        }

        return -1;
    }

    public static int LastIndexOfDifferent<T>(this T[][] list, T element, int column)
    {
        int i = list.Length -1;
        for (; i > -1; i--)
        {
            if (!Equals(list[i][column], element)) return i;
        }

        return -1;
    }


}
