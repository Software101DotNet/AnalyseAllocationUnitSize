# AnalyseAllocationUnitSize
AnalyseAUS is a program to analyse the range of file sizes of a folder or an entire Volume so that the optimal Allocation Unit Size value can be calculated.
 
Formatting a hard drive (or more specifically a Volume of a storage medium) in Windows, the default file Allocation Unit Size (AUS) is 4096 bytes. However, 4KB is not the only AUS option, and depending on the file sizes that the Volume may be used to store, it may not be the best choice for optimal performance of the storage medium.
 
Without going into the technical explanation of how FAT, FATex and NTFS file systems work, it suffices to say that Allocation Unit Size is a trade-off between file access performance vs file storage space used.
 
For Example, let's say we have a 130KB file.
 
With an AUS of 128KB, the 130KB file would require a cluster made up of 2 allocations of 128KB units of space on disk, resulting in a cluster totalling 256KB of allocated space on disk and two entries in the Volume's File Allocation Table to track them. However, of the 256KB allocated cluster, only 130KB is used for the file with 126KB of it unused.
 
If the AUS were 4KB instead of 128KB, the 130KB file would require a cluster made up of 33 blocks of 4KB (and 33 entries in the Volume's File Allocation Table to track them), totalling 132KB on disk. Of this cluster of 4KB blocks, 130KB is used for the file with only 2KB being unused allocation.
 
In this 130KB file example, it is more space-efficient on disk to use an AUS of 4KB instead of 128KB, but the file access is slower because 33 entries need to be managed for an AUS of 4KB instead of just 2 entries for an AUS of 128KB. Hence the trade-off between less file IO operations vs space efficiency.


Further reading:
    google: NTFS Allocation Unit Size
