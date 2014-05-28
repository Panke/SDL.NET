NAME = SDLTest.dll
VERSION = 0.1.0

SRC = $(wildcard *.cs)
SRC += $(wildcard ../SDL/*.cs)
TARGET = library
REFS = \
		 -r:NSubstitute.dll \
		 -r:nunit.framework.dll

DEFAULT: all

#RES_OPT = -resource:resources/$(BASE_NAME).resources,$(BASE_NAME).resources
#RES_OPT = -res:resources/mlRound32.ico,ml.ico -res:resources/ml.png,ml.png

PKG = $(wildcard $(BIN)/*.dll) $(BIN)/$(NAME).log4net
PKG_SRC = $(PKG) README.md ORDERS.md makefile $(wildcard test/*.cs) test/makefile

#################
BIN = bin
CSC = dmcs
#-nologo
NUNIT = ~/nunit
CSCFLAGS += -debug -nologo -target:$(TARGET)
CSCFLAGS += -lib:$(BIN)
CSCFLAGS += $(RES_OPT)
BASE_NAME = $(basename $(NAME))

PUBLISH_DIR = $(CS_DIR)/lib/Microline/$(BASE_NAME)/$(VERSION)
PKG_PREFIX = $(BASE_NAME)-$(VERSION)
PKG_DIR = pkg/$(PKG_PREFIX)
.PHONY: all clean clobber test alltest pkg pkgsrc publish

all: builddir $(BIN)/$(NAME)

builddir:
	@mkdir -p $(BIN)

pkgdir:
	@mkdir -p $(PKG_DIR)

pkg: $(PKG) | pkgdir
	cp $(PKG) --parents $(PKG_DIR)
	tar -jcf $(PKG_DIR).tar.bz2 --directory pkg $(PKG_PREFIX)/
	zip $(PKG_DIR).zip $(PKG)

pkgsrc: $(PKG_SRC) | pkgdir
	tar -jcf pkg/$(BASE_NAME)-$(VERSION)-src.tar.bz2 $^

$(BIN)/$(NAME): $(SRC) | builddir
	$(CSC) $(CSCFLAGS) $(REFS) -out:$@ $^
	
test: all
	$(NUNIT) $(BIN)/$(NAME) $(TF)

$(BIN)/$(AUT): ../$(BIN)/$(AUT)
	cp $^ $@ 

publishdir:
	@mkdir -p $(PUBLISH_DIR)

publish: publishdir
	cp -u --verbose --backup=t --preserve=all $(BIN)/$(NAME) $(PUBLISH_DIR)

tags: $(SRC)
	ctags $^

ver:
	@echo $(VERSION)

clean:
	-rm -f $(BIN)/$(NAME)

clobber:
	-rm -Rf $(BIN)

#include i18n.makefile
