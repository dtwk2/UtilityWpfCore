﻿using System;
using System.Runtime.InteropServices;


#nullable enable
namespace Utility.FileSystem.Helper
{
    public static class NativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool MoveFileWithProgress(
          string lpExistingFileName,
          string lpNewFileName,
          NativeMethods.CopyProgressRoutine lpProgressRoutine,
          IntPtr lpData,
          NativeMethods.MoveFileFlags dwCopyFlags);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CopyFileEx(
          string lpExistingFileName,
          string lpNewFileName,
          NativeMethods.CopyProgressRoutine lpProgressRoutine,
          IntPtr lpData,
          ref int pbCancel,
          NativeMethods.CopyFileFlags dwCopyFlags);

        public delegate NativeMethods.CopyProgressResult CopyProgressRoutine(
          long TotalFileSize,
          long TotalBytesTransferred,
          long StreamSize,
          long StreamBytesTransferred,
          uint dwStreamNumber,
          NativeMethods.CopyProgressCallbackReason dwCallbackReason,
          IntPtr hSourceFile,
          IntPtr hDestinationFile,
          IntPtr lpData);

        [Flags]
        public enum CopyFileFlags : uint
        {
            COPY_FILE_FAIL_IF_EXISTS = 1,
            COPY_FILE_RESTARTABLE = 2,
            COPY_FILE_OPEN_SOURCE_FOR_WRITE = 4,
            COPY_FILE_ALLOW_DECRYPTED_DESTINATION = 8,
            COPY_FILE_COPY_SYMLINK = 2048, // 0x00000800
        }

        public enum CopyProgressCallbackReason : uint
        {
            CALLBACK_CHUNK_FINISHED,
            CALLBACK_STREAM_SWITCH,
        }

        public enum CopyProgressResult : uint
        {
            PROGRESS_CONTINUE,
            PROGRESS_CANCEL,
            PROGRESS_STOP,
            PROGRESS_QUIET,
        }

        [Flags]
        public enum MoveFileFlags : uint
        {
            MOVE_FILE_REPLACE_EXISTSING = 1,
            MOVE_FILE_COPY_ALLOWED = 2,
            MOVE_FILE_DELAY_UNTIL_REBOOT = 4,
            MOVE_FILE_WRITE_THROUGH = 8,
            MOVE_FILE_CREATE_HARDLINK = 16, // 0x00000010
            MOVE_FILE_FAIL_IF_NOT_TRACKABLE = 32, // 0x00000020
        }
    }
}
