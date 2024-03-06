using System;

namespace Containers;

/// <summary>
/// A bi-partite circular buffer, for space restricted queueing.
/// This keeps each item in a contiguous memory space.
/// </summary>
/// <remarks>Based on https://www.codeproject.com/Articles/3479/The-Bip-Buffer-The-Circular-Buffer-with-a-Twist</remarks>
public class BiPartiteQueue
{
    /// <summary>
    /// Shared buffer for all elements
    /// </summary>
    public readonly byte[] Buffer;
    
    private int _idxA; // index of A
    private int _sizeA; // size of A
    private int _idxB; // index of B
    private int _sizeB; // size of B
    private int _idxReserved; // index of reservation
    private int _sizeReserved; // size of reservation

    /// <summary>
    /// Create a new queue with a fixed-size buffer
    /// </summary>
    public BiPartiteQueue(int bufferSize)
    {
        if (bufferSize <= 0) throw new ArgumentException(nameof(bufferSize));
        
        Buffer = new byte[bufferSize];
    }
    
    /// <summary>
    /// Clears the buffer of any allocations or reservations.
    /// Note: this does not wipe the buffer memory; it merely resets all pointers,
    /// returning the buffer to a completely empty state ready for new allocations.
    /// </summary>
    public void Clear()
    {
        _idxA = _sizeA = _idxB = _sizeB = _idxReserved = _sizeReserved = 0;
    }

    /// <summary>
    /// Reserves space in the buffer for a memory write operation.
    /// You can reserve more space than you need, and then only <see cref="Commit"/> what is used.
    /// You should always <see cref="Commit"/> after a Reserve.
    /// <p/>
    /// Will return false if space cannot be allocated.
    /// Will return false if a previous reservation has not been committed.
    /// </summary>
    /// <param name="size">amount of space to reserve</param>
    /// <param name="start">first index of reserved space</param>
    /// <param name="end">next index after end of reserved space</param>
    public bool Reserve(int size, out int start, out int end)
    {
        start = -1; end = -1;
        // We always allocate on B if B exists; this means we have two blocks and our buffer is filling.
        if (_sizeB != 0)
        {
            int freeSpace = GetBFreeSpace();
  
            if (size < freeSpace) freeSpace = size;
  
            if (freeSpace < size) return false;
  
            _sizeReserved = freeSpace;
            _idxReserved = _idxB + _sizeB;
            
            start = _idxReserved;
            end = start + size;
            return true;
        }
        else
        {
            // Block b does not exist, so we can check if the space AFTER a is bigger than the space
            // before A, and allocate the bigger one.
  
            int freeSpace = GetSpaceAfterA();
            if (freeSpace >= _idxA)
            {
                if (size < freeSpace) freeSpace = size;
                if (freeSpace < size) return false;
  
                _sizeReserved = freeSpace;
                _idxReserved = _idxA + _sizeA;
                
                start = _idxReserved;
                end = start + size;
                
                return true;
            }
            else
            {
                if (_idxA == 0) return false;
                if (_idxA < size) size = _idxA;
                _sizeReserved = size;
                _idxReserved = 0;
                
                start = _idxReserved;
                end = start + size;
                
                return true;
            }
        }
    }
    
    /// <summary>
    /// Commits space that has been written to in the buffer, after a <see cref="Reserve"/>
    /// <p/>
    /// Committing a size greater than the reserved size will cause an exception.
    /// Committing a size less than the reserved size will commit that amount of data, and release the rest of the space.
    /// Committing a size of 0 will release the reservation.
    /// </summary>
    /// <param name="size">number of bytes to commit</param>
    public void Commit(int size)
    {
        if (size == 0)
        {
            // de-commit any reservation
            _sizeReserved = _idxReserved = 0;
            return;
        }
  
        // If we try to commit more space than we asked for, throw an exception
        if (size > _sizeReserved) throw new InvalidOperationException("Attempted to commit more memory than reserved");
  
        // If we have no blocks being used currently, we create one in A.
        if (_sizeA == 0 && _sizeB == 0)
        {
            _idxA = _idxReserved;
            _sizeA = size;
  
            _idxReserved = 0;
            _sizeReserved = 0;
            return;
        }
  
        if (_idxReserved == _sizeA + _idxA) _sizeA += size;
        else _sizeB += size;
  
        _idxReserved = 0;
        _sizeReserved = 0;
    }
  
    /// <summary>
    /// Gets a pointer to the first contiguous block in the buffer, and returns the size of that block.
    /// </summary>
    /// <param name="start">first index of the block</param>
    /// <param name="end">next index after end of the block</param>
    public bool GetContiguousBlock(out int start, out int end)
    {
        if (_sizeA == 0)
        {
            start = end = -1;
            return false;
        }
  
        var size = _sizeA;
        start = _idxA;
        end = start + size;
        return true;
  
    }
  
    /// <summary>
    /// De-commits space from the first contiguous block
    /// </summary>
    /// <param name="size">amount of memory to de-commit</param>
    public void DeCommitBlock(int size)
    {
        if (size >= _sizeA)
        {
            _idxA = _idxB;
            _sizeA = _sizeB;
            _idxB = 0;
            _sizeB = 0;
        }
        else
        {
            _sizeA -= size;
            _idxA += size;
        }
    }
    
    /// <summary>
    /// Returns total amount of committed data in the buffer.
    /// Note than fragmentation means <c>bufferSize - GetCommittedSize()</c> is not the usable free space.
    /// </summary>
    public int GetCommittedSize()
    {
        return _sizeA + _sizeB;
    }
    
    /// <summary>
    /// Returns number of bytes that have been reserved.
    /// A return value of 0 indicates that no space has been reserved.
    /// </summary>
    public int GetReservationSize()
    {
        return _sizeReserved;
    }
    
    /// <summary>
    /// Returns total size of the allocated backing buffer.
    /// </summary>
    public int GetBufferSize()
    {
        return Buffer.Length;
    }
    
    private int GetSpaceAfterA()
    {
        return Buffer.Length - _idxA - _sizeA;
    }
  
    private int GetBFreeSpace()
    {
        return _idxA - _idxB - _sizeB;
    }
}