
SETUP
---------------------------

In Program.cs, change the path of the .txt file to match where you put your monopoly_population.txt file

This algorithm runs several thousand games against every group in it's tournament to eliminate randomnness and allow good stratergies to be thoroughly tested. 
I made the program multi-threaded to make it play the games as fast as possible and this will most definitely push your computer to the max.
If you want to save your computer from being at 99% cpu loadgo to the Tournament.cs file and change the variables WORKERS and BATCH_SIZE down from 20 to something smaller such as 5 or 3

there are a couple of monopoly_population.txt files included:

- 162 -> the brains I presented in my video that got to generation 162, they are quite good at monopoly (as far as I can tell)
- champions -> an old set of brains from about generation 60 (bad players but not suicidal)
- a blank text file -> this will start a new population at generation 1

rename those to monopoly_population.txt if you would like to continue training them.

---------------------------