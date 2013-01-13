
EXTRA_DIST =  Makefile

# Warning: This is an automatically generated file, do not edit!

srcdir=.
top_srcdir=../..

include $(top_srcdir)/config.make

ifeq ($(CONFIG),DEBUG_X86)
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG;"
ASSEMBLY = bin/Debug/Thom.CrtorrentTests.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../../lib/Bencode/src/Thom.Bencode/bin/Debug/Thom.Bencode.dll
BUILD_DIR = bin/Debug

THOM_CRTORRENTTESTS_DLL_MDB_SOURCE=bin/Debug/Thom.CrtorrentTests.dll.mdb
THOM_CRTORRENTTESTS_DLL_MDB=$(BUILD_DIR)/Thom.CrtorrentTests.dll.mdb
THOM_BENCODE_DLL_SOURCE=../../lib/Bencode/src/Thom.Bencode/bin/Debug/Thom.Bencode.dll
THOM_BENCODE_DLL_MDB_SOURCE=../../lib/Bencode/src/Thom.Bencode/bin/Debug/Thom.Bencode.dll.mdb

endif

ifeq ($(CONFIG),RELEASE_X86)
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/Thom.CrtorrentTests.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../../lib/Bencode/src/Thom.Bencode/bin/Release/Thom.Bencode.dll
BUILD_DIR = bin/Release

THOM_CRTORRENTTESTS_DLL_MDB=
THOM_BENCODE_DLL_SOURCE=../../lib/Bencode/src/Thom.Bencode/bin/Debug/Thom.Bencode.dll
THOM_BENCODE_DLL_MDB_SOURCE=../../lib/Bencode/src/Thom.Bencode/bin/Debug/Thom.Bencode.dll.mdb

endif

AL=al
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(THOM_CRTORRENTTESTS_DLL_MDB) \
	$(THOM_BENCODE_DLL) \
	$(THOM_BENCODE_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(THOM_CRTORRENTTESTS_PC)  


RESGEN=resgen2

THOM_BENCODE_DLL = $(BUILD_DIR)/Thom.Bencode.dll
THOM_BENCODE_DLL_MDB = $(BUILD_DIR)/Thom.Bencode.dll.mdb
THOM_CRTORRENTTESTS_PC = $(BUILD_DIR)/thom.crtorrenttests.pc

FILES = ChunkTest.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	thom.crtorrenttests.pc.in 

REFERENCES =  \
	../../lib/Bencode/src/Thom.Bencode/bin/Debug/Thom.Bencode.dll \
	-pkg:mono-nunit \
	System

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

#Targets
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG)  $(top_srcdir)/config.make

include $(top_srcdir)/Makefile.include
#include $(srcdir)/custom-hooks.make



$(eval $(call emit-deploy-target,THOM_BENCODE_DLL))
$(eval $(call emit-deploy-target,THOM_BENCODE_DLL_MDB))
$(eval $(call emit-deploy-wrapper,THOM_CRTORRENTTESTS_PC,thom.crtorrenttests.pc))


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
	$(call cp,$(THOM_CRTORRENTTESTS_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(THOM_BENCODE_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(THOM_BENCODE_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	mkdir -p '$(DESTDIR)$(libdir)/pkgconfig'
	$(call cp,$(THOM_CRTORRENTTESTS_PC),$(DESTDIR)$(libdir)/pkgconfig)
	make post-install-local-hook prefix=$(prefix)

uninstall-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-uninstall-local-hook prefix=$(prefix)
	make uninstall-satellite-assemblies prefix=$(prefix)
	$(call rm,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(THOM_CRTORRENTTESTS_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(THOM_BENCODE_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(THOM_BENCODE_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(THOM_CRTORRENTTESTS_PC),$(DESTDIR)$(libdir)/pkgconfig)
	make post-uninstall-local-hook prefix=$(prefix)

install: install-local
uninstall: uninstall-local
clean: clean-local

include $(top_srcdir)/rules.make
