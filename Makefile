#defaut shell
SHELL=/bin/sh

.SUFFIXES: 
.SUFFIXES: .cs .dll

.DEFAULT_GOAL=all

#List of folders to search for targets
VPATH = src lib .

#List of compile dependencies
OBJECTS=

#Compiler
CSC=msc

#Ouput directory
OUTPUTDIR=bin


.PHONY : all
all : crtorrent


#build target
#make build
# runs the $(OUTPUTDIR) only once
crtorrent: 
	@echo "done"

.PHONY : clean 
clean: 
	-rm -rf bin/*
	

