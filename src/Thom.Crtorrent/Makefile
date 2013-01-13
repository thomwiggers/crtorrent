
EXTRA_DIST =  Makefile

# Warning: This is an automatically generated file, do not edit!

srcdir=.
top_srcdir=../..

include $(top_srcdir)/config.make

ifeq ($(CONFIG),DEBUG_X86)
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG;" "-main:Thom.Crtorrent.Program"
ASSEMBLY = bin/Debug/Thom.Crtorrent.exe
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = exe
PROJECT_REFERENCES =  \
	../../lib/Bencode/src/Thom.Bencode/bin/Debug/Thom.Bencode.dll
BUILD_DIR = bin/Debug

THOM_CRTORRENT_EXE_MDB_SOURCE=bin/Debug/Thom.Crtorrent.exe.mdb
THOM_CRTORRENT_EXE_MDB=$(BUILD_DIR)/Thom.Crtorrent.exe.mdb
THOM_BENCODE_DLL_SOURCE=../../lib/Bencode/src/Thom.Bencode/bin/Debug/Thom.Bencode.dll
THOM_BENCODE_DLL_MDB_SOURCE=../../lib/Bencode/src/Thom.Bencode/bin/Debug/Thom.Bencode.dll.mdb
THOM_BENCODE_DLL_MDB=$(BUILD_DIR)/Thom.Bencode.dll.mdb

endif

ifeq ($(CONFIG),RELEASE_X86)
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ "-main:Thom.Crtorrent.Program"
ASSEMBLY = bin/Release/Thom.Crtorrent.exe
ASSEMBLY_MDB = 
COMPILE_TARGET = exe
PROJECT_REFERENCES =  \
	../../lib/Bencode/src/Thom.Bencode/bin/Release/Thom.Bencode.dll
BUILD_DIR = bin/Release

THOM_CRTORRENT_EXE_MDB=
THOM_BENCODE_DLL_SOURCE=../../lib/Bencode/src/Thom.Bencode/bin/Release/Thom.Bencode.dll
THOM_BENCODE_DLL_MDB=

endif

AL=al
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(THOM_CRTORRENT_EXE_MDB) \
	$(THOM_BENCODE_DLL) \
	$(THOM_BENCODE_DLL_MDB)  

BINARIES = \
	$(CRTORRENT)  


RESGEN=resgen2

THOM_BENCODE_DLL = $(BUILD_DIR)/Thom.Bencode.dll
CRTORRENT = $(BUILD_DIR)/crtorrent

FILES =  \
	Chunk.cs \
	ChunkSource.cs \
	FatalException.cs \
	Hasher.cs \
	InvalidArgumentException.cs \
	Metafile.cs \
	Program.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	crtorrent.in 

REFERENCES =  \
	../../lib/Bencode/src/Thom.Bencode/bin/Debug/Thom.Bencode.dll \
	System \
	System.Core

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(BINARIES) 

#Targets
all: $(ASSEMBLY) $(PROGRAMFILES) $(BINARIES)  $(top_srcdir)/config.make

include $(top_srcdir)/Makefile.include
#include $(srcdir)/custom-hooks.make



$(eval $(call emit-deploy-target,THOM_BENCODE_DLL))
$(eval $(call emit-deploy-target,THOM_BENCODE_DLL_MDB))
$(eval $(call emit-deploy-wrapper,CRTORRENT,crtorrent,x))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'


$(ASSEMBLY_MDB): $(ASSEMBLY)
$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	make pre-all-local-hook prefix=$(prefix)
	mkdir -p $(shell dirname $(ASSEMBLY))
	make $(CONFIG)_BeforeBuild
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
	make $(CONFIG)_AfterBuild
	make post-all-local-hook prefix=$(prefix)

install-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-install-local-hook prefix=$(prefix)
	make install-satellite-assemblies prefix=$(prefix)
	mkdir -p '$(DESTDIR)$(libdir)/$(PACKAGE)'
	$(call cp,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(THOM_CRTORRENT_EXE_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(THOM_BENCODE_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(THOM_BENCODE_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	mkdir -p '$(DESTDIR)$(bindir)'
	$(call cp,$(CRTORRENT),$(DESTDIR)$(bindir))
	make post-install-local-hook prefix=$(prefix)

uninstall-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-uninstall-local-hook prefix=$(prefix)
	make uninstall-satellite-assemblies prefix=$(prefix)
	$(call rm,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(THOM_CRTORRENT_EXE_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(THOM_BENCODE_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(THOM_BENCODE_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(CRTORRENT),$(DESTDIR)$(bindir))
	make post-uninstall-local-hook prefix=$(prefix)

install: install-local
uninstall: uninstall-local
clean: clean-local

include $(top_srcdir)/rules.make
