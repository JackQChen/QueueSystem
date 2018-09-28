using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

public class ShareMemory
{

    [DllImport("kernel32.dll", EntryPoint = "OpenFileMapping", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern IntPtr OpenFileMapping(int dwDesiredAccess, bool bInheritHandle, String lpName);

    // 创建一个文件映射内核对象
    [DllImport("Kernel32.dll", EntryPoint = "CreateFileMapping", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern IntPtr CreateFileMapping(uint hFile, IntPtr lpAttributes, uint flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

    // 把文件数据映射到进程的地址空间
    [DllImport("Kernel32.dll")]
    private static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);

    [DllImport("Kernel32.dll", EntryPoint = "UnmapViewOfFile", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

    [DllImport("kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool CloseHandle(uint hHandle);

    [DllImport("kernel32.dll", EntryPoint = "GetLastError", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern uint GetLastError();

    private structPerson G_Person = new structPerson();//身份证的数据结构,共享内存方式
    private IntPtr G_MemoryAddress;//内存地址
    private IntPtr G_ProcessAddress;//进程地址
    private Person _person;//从共享内存读到的数据

    public ShareMemory()
    {

    }
    public Person person
    {
        get
        {
            return this._person;
        }
        set
        {
            this._person = value;
        }
    }


    private struct structPerson
    {
        public Person person;
    }
    public bool ReadMemory()
    {
        try
        {
            G_Person.person = null;
            if (G_MemoryAddress == IntPtr.Zero)
                G_MemoryAddress = CreateFileMapping(0xFFFFFFFF, IntPtr.Zero, (uint)0, 0, (uint)0x1024 * 10, "person");
            if (G_ProcessAddress == IntPtr.Zero)
                G_ProcessAddress = MapViewOfFile(this.G_MemoryAddress, 0xf001f, 0, 0, 0);
            var num = G_ProcessAddress.ToInt32() + 0x1024 * 10;
            G_Person = (structPerson)Marshal.PtrToStructure((IntPtr)(num), typeof(structPerson));
            this._person = G_Person.person;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void WriterMemory(Person person)
    {
        try
        {
            structPerson s_person = new structPerson();
            s_person.person = person;
            if (this.G_MemoryAddress == IntPtr.Zero)
                this.G_MemoryAddress = CreateFileMapping(0xFFFFFFFF, IntPtr.Zero, 0, 0, (uint)0x1024 * 10, "person");
            if (this.G_ProcessAddress == IntPtr.Zero)
                this.G_ProcessAddress = MapViewOfFile(this.G_MemoryAddress, 0xf001f, 0, 0, 0);
            var num = G_ProcessAddress.ToInt32() + 0x1024 * 10;
            Marshal.StructureToPtr(s_person, (IntPtr)(num), true);
        }
        catch (Exception exception)
        {
            throw exception;
        }
    }

    public void ClearMemory()
    {
        try
        {
            G_Person.person = null;
            if (G_MemoryAddress == IntPtr.Zero)
                G_MemoryAddress = CreateFileMapping(0xFFFFFFFF, IntPtr.Zero, (uint)0, 0, (uint)0x1024 * 10, "person");
            if (G_ProcessAddress == IntPtr.Zero)
                G_ProcessAddress = MapViewOfFile(G_MemoryAddress, 0xf001f, 0, 0, 0);
            var num = G_ProcessAddress.ToInt32() + 0x1024 * 10;
            Marshal.StructureToPtr(G_Person, (IntPtr)(num), true);
        }
        catch (System.Exception exception)
        {
            throw (exception);
        }
    }
}

public class ShareMemory2
{
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

    [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr CreateFileMapping(int hFile, IntPtr lpAttributes, uint flProtect, uint dwMaxSizeHi, uint dwMaxSizeLow, string lpName);

    [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr OpenFileMapping(int dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, string lpName);

    [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr MapViewOfFile(IntPtr hFileMapping, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);

    [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
    public static extern bool UnmapViewOfFile(IntPtr pvBaseAddress);

    [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
    public static extern bool CloseHandle(IntPtr handle);

    [DllImport("kernel32", EntryPoint = "GetLastError")]
    public static extern int GetLastError();
    const int ERROR_ALREADY_EXISTS = 183;
    const int FILE_MAP_COPY = 0x0001;
    const int FILE_MAP_WRITE = 0x0002;
    const int FILE_MAP_READ = 0x0004;
    const int FILE_MAP_ALL_ACCESS = 0x0002 | 0x0004;
    const int PAGE_READONLY = 0x02;
    const int PAGE_READWRITE = 0x04;
    const int PAGE_WRITECOPY = 0x08;
    const int PAGE_EXECUTE = 0x10;
    const int PAGE_EXECUTE_READ = 0x20;
    const int PAGE_EXECUTE_READWRITE = 0x40;
    const int SEC_COMMIT = 0x8000000;
    const int SEC_IMAGE = 0x1000000;
    const int SEC_NOCACHE = 0x10000000;
    const int SEC_RESERVE = 0x4000000;
    const int INVALID_HANDLE_VALUE = -1;
    IntPtr m_hSharedMemoryFile = IntPtr.Zero;
    IntPtr m_pwData = IntPtr.Zero;
    bool m_bAlreadyExist = false;
    bool m_bInit = false;
    long m_MemSize = 0;

    public ShareMemory2()
    {
    }

    /// <summary>
    /// 初始化共享内存
    /// </summary>
    /// <param name="strName">共享内存名称</param>
    /// <returns></returns>
    public int Init()
    {
        long lngSize = 1024 * 10;
        if (lngSize <= 0 || lngSize > 0x00800000) lngSize = 0x00800000;
        m_MemSize = lngSize;
        //创建内存共享体(INVALID_HANDLE_VALUE)
        m_hSharedMemoryFile = CreateFileMapping(INVALID_HANDLE_VALUE, IntPtr.Zero, (uint)PAGE_READWRITE, 0, (uint)lngSize, "person");
        if (m_hSharedMemoryFile == IntPtr.Zero)
        {
            m_bAlreadyExist = false;
            m_bInit = false;
            return 2; //创建共享体失败
        }
        else
        {
            if (GetLastError() == ERROR_ALREADY_EXISTS)  //已经创建
            {
                m_bAlreadyExist = true;
            }
            else                                         //新创建
            {
                m_bAlreadyExist = false;
            }
        }
        //---------------------------------------
        //创建内存映射
        m_pwData = MapViewOfFile(m_hSharedMemoryFile, FILE_MAP_WRITE, 0, 0, (uint)lngSize);
        if (m_pwData == IntPtr.Zero)
        {
            m_bInit = false;
            CloseHandle(m_hSharedMemoryFile);
            return 3; //创建内存映射失败
        }
        else
        {
            m_bInit = true;
            if (m_bAlreadyExist == false)
            {
                //初始化
            }
        }
        //----------------------------------------

        return 0;     //创建成功
    }
    /// <summary>
    /// 关闭共享内存
    /// </summary>
    public void Close()
    {
        if (m_bInit)
        {
            UnmapViewOfFile(m_pwData);
            CloseHandle(m_hSharedMemoryFile);
        }
    }

    /// <summary>
    /// 读数据
    /// </summary>
    /// <param name="bytData">数据</param>
    /// <param name="lngAddr">起始地址</param>
    /// <param name="lngSize">个数</param>
    /// <returns></returns>
    public bool Read(ref byte[] bytData, int lngAddr, int lngSize)
    {
        if (lngAddr + lngSize > m_MemSize) return false; //超出数据区
        if (m_bInit)
        {
            Marshal.Copy(m_pwData, bytData, lngAddr, lngSize);
        }
        else
        {
            return false; //共享内存未初始化
        }
        return true;     //读成功
    }

    /// <summary>
    /// 写数据
    /// </summary>
    /// <param name="bytData">数据</param>
    /// <param name="lngAddr">起始地址</param>
    /// <param name="lngSize">个数</param>
    /// <returns></returns>
    public bool Write(byte[] bytData, int lngAddr, int lngSize)
    {
        if (lngAddr + lngSize > m_MemSize) return false; //超出数据区
        if (m_bInit)
        {
            Marshal.Copy(bytData, lngAddr, m_pwData, lngSize);
        }
        else
        {
            return false; //共享内存未初始化
        }
        return true;     //写成功
    }
    public bool BytesCompare(byte[] b1, byte[] b2)
    {
        if (b1 == null || b2 == null) return false;
        if (b1.Length != b2.Length) return false;
        return string.Compare(Convert.ToBase64String(b1), Convert.ToBase64String(b2), false) == 0 ? true : false;
    }
}
