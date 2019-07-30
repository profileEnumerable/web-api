// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Pluralization.EnglishPluralizationService
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Design.PluralizationServices;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Data.Entity.Infrastructure.Pluralization
{
  /// <summary>
  /// Default pluralization service implementation to be used by Entity Framework. This pluralization
  /// service is based on English locale.
  /// </summary>
  [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pluralization")]
  public sealed class EnglishPluralizationService : IPluralizationService
  {
    private readonly CultureInfo _culture = new CultureInfo("en-US");
    private readonly string[] _uninflectiveSuffixes = new string[7]
    {
      "fish",
      "ois",
      "sheep",
      "deer",
      "pos",
      "itis",
      "ism"
    };
    private readonly string[] _uninflectiveWords = new string[84]
    {
      "bison",
      "flounder",
      "pliers",
      "bream",
      "gallows",
      "proceedings",
      "breeches",
      "graffiti",
      "rabies",
      "britches",
      "headquarters",
      "salmon",
      "carp",
      "herpes",
      "scissors",
      "chassis",
      "high-jinks",
      "sea-bass",
      "clippers",
      "homework",
      "series",
      "cod",
      "innings",
      "shears",
      "contretemps",
      "jackanapes",
      "species",
      "corps",
      "mackerel",
      "swine",
      "debris",
      "measles",
      "trout",
      "diabetes",
      "mews",
      "tuna",
      "djinn",
      "mumps",
      "whiting",
      "eland",
      "news",
      "wildebeest",
      "elk",
      "pincers",
      "police",
      "hair",
      "ice",
      "chaos",
      "milk",
      "cotton",
      "corn",
      "millet",
      "hay",
      "pneumonoultramicroscopicsilicovolcanoconiosis",
      "information",
      "rice",
      "tobacco",
      "aircraft",
      "rabies",
      "scabies",
      "diabetes",
      "traffic",
      "cotton",
      "corn",
      "millet",
      "rice",
      "hay",
      "hemp",
      "tobacco",
      "cabbage",
      "okra",
      "broccoli",
      "asparagus",
      "lettuce",
      "beef",
      "pork",
      "venison",
      "bison",
      "mutton",
      "cattle",
      "offspring",
      "molasses",
      "shambles",
      "shingles"
    };
    private readonly Dictionary<string, string> _irregularVerbList = new Dictionary<string, string>()
    {
      {
        "am",
        "are"
      },
      {
        "are",
        "are"
      },
      {
        "is",
        "are"
      },
      {
        "was",
        "were"
      },
      {
        "were",
        "were"
      },
      {
        "has",
        "have"
      },
      {
        "have",
        "have"
      }
    };
    private readonly List<string> _pronounList = new List<string>()
    {
      "I",
      "we",
      "you",
      "he",
      "she",
      "they",
      "it",
      "me",
      "us",
      "him",
      "her",
      "them",
      "myself",
      "ourselves",
      "yourself",
      "himself",
      "herself",
      "itself",
      "oneself",
      "oneselves",
      "my",
      "our",
      "your",
      "his",
      "their",
      "its",
      "mine",
      "yours",
      "hers",
      "theirs",
      "this",
      "that",
      "these",
      "those",
      "all",
      "another",
      "any",
      "anybody",
      "anyone",
      "anything",
      "both",
      "each",
      "other",
      "either",
      "everyone",
      "everybody",
      "everything",
      "most",
      "much",
      "nothing",
      "nobody",
      "none",
      "one",
      "others",
      "some",
      "somebody",
      "someone",
      "something",
      "what",
      "whatever",
      "which",
      "whichever",
      "who",
      "whoever",
      "whom",
      "whomever",
      "whose"
    };
    private readonly Dictionary<string, string> _irregularPluralsList = new Dictionary<string, string>()
    {
      {
        "brother",
        "brothers"
      },
      {
        "child",
        "children"
      },
      {
        "cow",
        "cows"
      },
      {
        "ephemeris",
        "ephemerides"
      },
      {
        "genie",
        "genies"
      },
      {
        "money",
        "moneys"
      },
      {
        "mongoose",
        "mongooses"
      },
      {
        "mythos",
        "mythoi"
      },
      {
        "octopus",
        "octopuses"
      },
      {
        "ox",
        "oxen"
      },
      {
        "soliloquy",
        "soliloquies"
      },
      {
        "trilby",
        "trilbys"
      },
      {
        "crisis",
        "crises"
      },
      {
        "synopsis",
        "synopses"
      },
      {
        "rose",
        "roses"
      },
      {
        "gas",
        "gases"
      },
      {
        "bus",
        "buses"
      },
      {
        "axis",
        "axes"
      },
      {
        "memo",
        "memos"
      },
      {
        "casino",
        "casinos"
      },
      {
        "silo",
        "silos"
      },
      {
        "stereo",
        "stereos"
      },
      {
        "studio",
        "studios"
      },
      {
        "lens",
        "lenses"
      },
      {
        "alias",
        "aliases"
      },
      {
        "pie",
        "pies"
      },
      {
        "corpus",
        "corpora"
      },
      {
        "viscus",
        "viscera"
      },
      {
        "hippopotamus",
        "hippopotami"
      },
      {
        "trace",
        "traces"
      },
      {
        "person",
        "people"
      },
      {
        "chilli",
        "chillies"
      },
      {
        "analysis",
        "analyses"
      },
      {
        "basis",
        "bases"
      },
      {
        "neurosis",
        "neuroses"
      },
      {
        "oasis",
        "oases"
      },
      {
        "synthesis",
        "syntheses"
      },
      {
        "thesis",
        "theses"
      },
      {
        "pneumonoultramicroscopicsilicovolcanoconiosis",
        "pneumonoultramicroscopicsilicovolcanoconioses"
      },
      {
        "status",
        "statuses"
      },
      {
        "prospectus",
        "prospectuses"
      },
      {
        "change",
        "changes"
      },
      {
        "lie",
        "lies"
      },
      {
        "calorie",
        "calories"
      },
      {
        "freebie",
        "freebies"
      },
      {
        "case",
        "cases"
      },
      {
        "house",
        "houses"
      },
      {
        "valve",
        "valves"
      },
      {
        "cloth",
        "clothes"
      }
    };
    private readonly Dictionary<string, string> _assimilatedClassicalInflectionList = new Dictionary<string, string>()
    {
      {
        "alumna",
        "alumnae"
      },
      {
        "alga",
        "algae"
      },
      {
        "vertebra",
        "vertebrae"
      },
      {
        "codex",
        "codices"
      },
      {
        "murex",
        "murices"
      },
      {
        "silex",
        "silices"
      },
      {
        "aphelion",
        "aphelia"
      },
      {
        "hyperbaton",
        "hyperbata"
      },
      {
        "perihelion",
        "perihelia"
      },
      {
        "asyndeton",
        "asyndeta"
      },
      {
        "noumenon",
        "noumena"
      },
      {
        "phenomenon",
        "phenomena"
      },
      {
        "criterion",
        "criteria"
      },
      {
        "organon",
        "organa"
      },
      {
        "prolegomenon",
        "prolegomena"
      },
      {
        "agendum",
        "agenda"
      },
      {
        "datum",
        "data"
      },
      {
        "extremum",
        "extrema"
      },
      {
        "bacterium",
        "bacteria"
      },
      {
        "desideratum",
        "desiderata"
      },
      {
        "stratum",
        "strata"
      },
      {
        "candelabrum",
        "candelabra"
      },
      {
        "erratum",
        "errata"
      },
      {
        "ovum",
        "ova"
      },
      {
        "forum",
        "fora"
      },
      {
        "addendum",
        "addenda"
      },
      {
        "stadium",
        "stadia"
      },
      {
        "automaton",
        "automata"
      },
      {
        "polyhedron",
        "polyhedra"
      }
    };
    private readonly Dictionary<string, string> _oSuffixList = new Dictionary<string, string>()
    {
      {
        "albino",
        "albinos"
      },
      {
        "generalissimo",
        "generalissimos"
      },
      {
        "manifesto",
        "manifestos"
      },
      {
        "archipelago",
        "archipelagos"
      },
      {
        "ghetto",
        "ghettos"
      },
      {
        "medico",
        "medicos"
      },
      {
        "armadillo",
        "armadillos"
      },
      {
        "guano",
        "guanos"
      },
      {
        "octavo",
        "octavos"
      },
      {
        "commando",
        "commandos"
      },
      {
        "inferno",
        "infernos"
      },
      {
        "photo",
        "photos"
      },
      {
        "ditto",
        "dittos"
      },
      {
        "jumbo",
        "jumbos"
      },
      {
        "pro",
        "pros"
      },
      {
        "dynamo",
        "dynamos"
      },
      {
        "lingo",
        "lingos"
      },
      {
        "quarto",
        "quartos"
      },
      {
        "embryo",
        "embryos"
      },
      {
        "lumbago",
        "lumbagos"
      },
      {
        "rhino",
        "rhinos"
      },
      {
        "fiasco",
        "fiascos"
      },
      {
        "magneto",
        "magnetos"
      },
      {
        "stylo",
        "stylos"
      }
    };
    private readonly Dictionary<string, string> _classicalInflectionList = new Dictionary<string, string>()
    {
      {
        "stamen",
        "stamina"
      },
      {
        "foramen",
        "foramina"
      },
      {
        "lumen",
        "lumina"
      },
      {
        "anathema",
        "anathemata"
      },
      {
        "enema",
        "enemata"
      },
      {
        "oedema",
        "oedemata"
      },
      {
        "bema",
        "bemata"
      },
      {
        "enigma",
        "enigmata"
      },
      {
        "sarcoma",
        "sarcomata"
      },
      {
        "carcinoma",
        "carcinomata"
      },
      {
        "gumma",
        "gummata"
      },
      {
        "schema",
        "schemata"
      },
      {
        "charisma",
        "charismata"
      },
      {
        "lemma",
        "lemmata"
      },
      {
        "soma",
        "somata"
      },
      {
        "diploma",
        "diplomata"
      },
      {
        "lymphoma",
        "lymphomata"
      },
      {
        "stigma",
        "stigmata"
      },
      {
        "dogma",
        "dogmata"
      },
      {
        "magma",
        "magmata"
      },
      {
        "stoma",
        "stomata"
      },
      {
        "drama",
        "dramata"
      },
      {
        "melisma",
        "melismata"
      },
      {
        "trauma",
        "traumata"
      },
      {
        "edema",
        "edemata"
      },
      {
        "miasma",
        "miasmata"
      },
      {
        "abscissa",
        "abscissae"
      },
      {
        "formula",
        "formulae"
      },
      {
        "medusa",
        "medusae"
      },
      {
        "amoeba",
        "amoebae"
      },
      {
        "hydra",
        "hydrae"
      },
      {
        "nebula",
        "nebulae"
      },
      {
        "antenna",
        "antennae"
      },
      {
        "hyperbola",
        "hyperbolae"
      },
      {
        "nova",
        "novae"
      },
      {
        "aurora",
        "aurorae"
      },
      {
        "lacuna",
        "lacunae"
      },
      {
        "parabola",
        "parabolae"
      },
      {
        "apex",
        "apices"
      },
      {
        "latex",
        "latices"
      },
      {
        "vertex",
        "vertices"
      },
      {
        "cortex",
        "cortices"
      },
      {
        "pontifex",
        "pontifices"
      },
      {
        "vortex",
        "vortices"
      },
      {
        "index",
        "indices"
      },
      {
        "simplex",
        "simplices"
      },
      {
        "iris",
        "irides"
      },
      {
        "clitoris",
        "clitorides"
      },
      {
        "alto",
        "alti"
      },
      {
        "contralto",
        "contralti"
      },
      {
        "soprano",
        "soprani"
      },
      {
        "basso",
        "bassi"
      },
      {
        "crescendo",
        "crescendi"
      },
      {
        "tempo",
        "tempi"
      },
      {
        "canto",
        "canti"
      },
      {
        "solo",
        "soli"
      },
      {
        "aquarium",
        "aquaria"
      },
      {
        "interregnum",
        "interregna"
      },
      {
        "quantum",
        "quanta"
      },
      {
        "compendium",
        "compendia"
      },
      {
        "lustrum",
        "lustra"
      },
      {
        "rostrum",
        "rostra"
      },
      {
        "consortium",
        "consortia"
      },
      {
        "maximum",
        "maxima"
      },
      {
        "spectrum",
        "spectra"
      },
      {
        "cranium",
        "crania"
      },
      {
        "medium",
        "media"
      },
      {
        "speculum",
        "specula"
      },
      {
        "curriculum",
        "curricula"
      },
      {
        "memorandum",
        "memoranda"
      },
      {
        "stadium",
        "stadia"
      },
      {
        "dictum",
        "dicta"
      },
      {
        "millenium",
        "millenia"
      },
      {
        "trapezium",
        "trapezia"
      },
      {
        "emporium",
        "emporia"
      },
      {
        "minimum",
        "minima"
      },
      {
        "ultimatum",
        "ultimata"
      },
      {
        "enconium",
        "enconia"
      },
      {
        "momentum",
        "momenta"
      },
      {
        "vacuum",
        "vacua"
      },
      {
        "gymnasium",
        "gymnasia"
      },
      {
        "optimum",
        "optima"
      },
      {
        "velum",
        "vela"
      },
      {
        "honorarium",
        "honoraria"
      },
      {
        "phylum",
        "phyla"
      },
      {
        "focus",
        "foci"
      },
      {
        "nimbus",
        "nimbi"
      },
      {
        "succubus",
        "succubi"
      },
      {
        "fungus",
        "fungi"
      },
      {
        "nucleolus",
        "nucleoli"
      },
      {
        "torus",
        "tori"
      },
      {
        "genius",
        "genii"
      },
      {
        "radius",
        "radii"
      },
      {
        "umbilicus",
        "umbilici"
      },
      {
        "incubus",
        "incubi"
      },
      {
        "stylus",
        "styli"
      },
      {
        "uterus",
        "uteri"
      },
      {
        "stimulus",
        "stimuli"
      },
      {
        "apparatus",
        "apparatus"
      },
      {
        "impetus",
        "impetus"
      },
      {
        "prospectus",
        "prospectus"
      },
      {
        "cantus",
        "cantus"
      },
      {
        "nexus",
        "nexus"
      },
      {
        "sinus",
        "sinus"
      },
      {
        "coitus",
        "coitus"
      },
      {
        "plexus",
        "plexus"
      },
      {
        "status",
        "status"
      },
      {
        "hiatus",
        "hiatus"
      },
      {
        "afreet",
        "afreeti"
      },
      {
        "afrit",
        "afriti"
      },
      {
        "efreet",
        "efreeti"
      },
      {
        "cherub",
        "cherubim"
      },
      {
        "goy",
        "goyim"
      },
      {
        "seraph",
        "seraphim"
      },
      {
        "alumnus",
        "alumni"
      }
    };
    private readonly List<string> _knownConflictingPluralList = new List<string>()
    {
      "they",
      "them",
      "their",
      "have",
      "were",
      "yourself",
      "are"
    };
    private readonly Dictionary<string, string> _wordsEndingWithSeList = new Dictionary<string, string>()
    {
      {
        "house",
        "houses"
      },
      {
        "case",
        "cases"
      },
      {
        "enterprise",
        "enterprises"
      },
      {
        "purchase",
        "purchases"
      },
      {
        "surprise",
        "surprises"
      },
      {
        "release",
        "releases"
      },
      {
        "disease",
        "diseases"
      },
      {
        "promise",
        "promises"
      },
      {
        "refuse",
        "refuses"
      },
      {
        "whose",
        "whoses"
      },
      {
        "phase",
        "phases"
      },
      {
        "noise",
        "noises"
      },
      {
        "nurse",
        "nurses"
      },
      {
        "rose",
        "roses"
      },
      {
        "franchise",
        "franchises"
      },
      {
        "supervise",
        "supervises"
      },
      {
        "farmhouse",
        "farmhouses"
      },
      {
        "suitcase",
        "suitcases"
      },
      {
        "recourse",
        "recourses"
      },
      {
        "impulse",
        "impulses"
      },
      {
        "license",
        "licenses"
      },
      {
        "diocese",
        "dioceses"
      },
      {
        "excise",
        "excises"
      },
      {
        "demise",
        "demises"
      },
      {
        "blouse",
        "blouses"
      },
      {
        "bruise",
        "bruises"
      },
      {
        "misuse",
        "misuses"
      },
      {
        "curse",
        "curses"
      },
      {
        "prose",
        "proses"
      },
      {
        "purse",
        "purses"
      },
      {
        "goose",
        "gooses"
      },
      {
        "tease",
        "teases"
      },
      {
        "poise",
        "poises"
      },
      {
        "vase",
        "vases"
      },
      {
        "fuse",
        "fuses"
      },
      {
        "muse",
        "muses"
      },
      {
        "slaughterhouse",
        "slaughterhouses"
      },
      {
        "clearinghouse",
        "clearinghouses"
      },
      {
        "endonuclease",
        "endonucleases"
      },
      {
        "steeplechase",
        "steeplechases"
      },
      {
        "metamorphose",
        "metamorphoses"
      },
      {
        "intercourse",
        "intercourses"
      },
      {
        "commonsense",
        "commonsenses"
      },
      {
        "intersperse",
        "intersperses"
      },
      {
        "merchandise",
        "merchandises"
      },
      {
        "phosphatase",
        "phosphatases"
      },
      {
        "summerhouse",
        "summerhouses"
      },
      {
        "watercourse",
        "watercourses"
      },
      {
        "catchphrase",
        "catchphrases"
      },
      {
        "compromise",
        "compromises"
      },
      {
        "greenhouse",
        "greenhouses"
      },
      {
        "lighthouse",
        "lighthouses"
      },
      {
        "paraphrase",
        "paraphrases"
      },
      {
        "mayonnaise",
        "mayonnaises"
      },
      {
        "racecourse",
        "racecourses"
      },
      {
        "apocalypse",
        "apocalypses"
      },
      {
        "courthouse",
        "courthouses"
      },
      {
        "powerhouse",
        "powerhouses"
      },
      {
        "storehouse",
        "storehouses"
      },
      {
        "glasshouse",
        "glasshouses"
      },
      {
        "hypotenuse",
        "hypotenuses"
      },
      {
        "peroxidase",
        "peroxidases"
      },
      {
        "pillowcase",
        "pillowcases"
      },
      {
        "roundhouse",
        "roundhouses"
      },
      {
        "streetwise",
        "streetwises"
      },
      {
        "expertise",
        "expertises"
      },
      {
        "discourse",
        "discourses"
      },
      {
        "warehouse",
        "warehouses"
      },
      {
        "staircase",
        "staircases"
      },
      {
        "workhouse",
        "workhouses"
      },
      {
        "briefcase",
        "briefcases"
      },
      {
        "clubhouse",
        "clubhouses"
      },
      {
        "clockwise",
        "clockwises"
      },
      {
        "concourse",
        "concourses"
      },
      {
        "playhouse",
        "playhouses"
      },
      {
        "turquoise",
        "turquoises"
      },
      {
        "boathouse",
        "boathouses"
      },
      {
        "cellulose",
        "celluloses"
      },
      {
        "epitomise",
        "epitomises"
      },
      {
        "gatehouse",
        "gatehouses"
      },
      {
        "grandiose",
        "grandioses"
      },
      {
        "menopause",
        "menopauses"
      },
      {
        "penthouse",
        "penthouses"
      },
      {
        "racehorse",
        "racehorses"
      },
      {
        "transpose",
        "transposes"
      },
      {
        "almshouse",
        "almshouses"
      },
      {
        "customise",
        "customises"
      },
      {
        "footloose",
        "footlooses"
      },
      {
        "galvanise",
        "galvanises"
      },
      {
        "princesse",
        "princesses"
      },
      {
        "universe",
        "universes"
      },
      {
        "workhorse",
        "workhorses"
      }
    };
    private readonly Dictionary<string, string> _wordsEndingWithSisList = new Dictionary<string, string>()
    {
      {
        "analysis",
        "analyses"
      },
      {
        "crisis",
        "crises"
      },
      {
        "basis",
        "bases"
      },
      {
        "atherosclerosis",
        "atheroscleroses"
      },
      {
        "electrophoresis",
        "electrophoreses"
      },
      {
        "psychoanalysis",
        "psychoanalyses"
      },
      {
        "photosynthesis",
        "photosyntheses"
      },
      {
        "amniocentesis",
        "amniocenteses"
      },
      {
        "metamorphosis",
        "metamorphoses"
      },
      {
        "toxoplasmosis",
        "toxoplasmoses"
      },
      {
        "endometriosis",
        "endometrioses"
      },
      {
        "tuberculosis",
        "tuberculoses"
      },
      {
        "pathogenesis",
        "pathogeneses"
      },
      {
        "osteoporosis",
        "osteoporoses"
      },
      {
        "parenthesis",
        "parentheses"
      },
      {
        "anastomosis",
        "anastomoses"
      },
      {
        "peristalsis",
        "peristalses"
      },
      {
        "hypothesis",
        "hypotheses"
      },
      {
        "antithesis",
        "antitheses"
      },
      {
        "apotheosis",
        "apotheoses"
      },
      {
        "thrombosis",
        "thromboses"
      },
      {
        "diagnosis",
        "diagnoses"
      },
      {
        "synthesis",
        "syntheses"
      },
      {
        "paralysis",
        "paralyses"
      },
      {
        "prognosis",
        "prognoses"
      },
      {
        "cirrhosis",
        "cirrhoses"
      },
      {
        "sclerosis",
        "scleroses"
      },
      {
        "psychosis",
        "psychoses"
      },
      {
        "apoptosis",
        "apoptoses"
      },
      {
        "symbiosis",
        "symbioses"
      }
    };
    private readonly BidirectionalDictionary<string, string> _userDictionary;
    private readonly StringBidirectionalDictionary _irregularPluralsPluralizationService;
    private readonly StringBidirectionalDictionary _assimilatedClassicalInflectionPluralizationService;
    private readonly StringBidirectionalDictionary _oSuffixPluralizationService;
    private readonly StringBidirectionalDictionary _classicalInflectionPluralizationService;
    private readonly StringBidirectionalDictionary _irregularVerbPluralizationService;
    private readonly StringBidirectionalDictionary _wordsEndingWithSePluralizationService;
    private readonly StringBidirectionalDictionary _wordsEndingWithSisPluralizationService;
    private readonly List<string> _knownSingluarWords;
    private readonly List<string> _knownPluralWords;

    /// <summary>
    /// Constructs a new  instance  of default pluralization service
    /// used in Entity Framework.
    /// </summary>
    public EnglishPluralizationService()
    {
      this._userDictionary = new BidirectionalDictionary<string, string>();
      this._irregularPluralsPluralizationService = new StringBidirectionalDictionary(this._irregularPluralsList);
      this._assimilatedClassicalInflectionPluralizationService = new StringBidirectionalDictionary(this._assimilatedClassicalInflectionList);
      this._oSuffixPluralizationService = new StringBidirectionalDictionary(this._oSuffixList);
      this._classicalInflectionPluralizationService = new StringBidirectionalDictionary(this._classicalInflectionList);
      this._wordsEndingWithSePluralizationService = new StringBidirectionalDictionary(this._wordsEndingWithSeList);
      this._wordsEndingWithSisPluralizationService = new StringBidirectionalDictionary(this._wordsEndingWithSisList);
      this._irregularVerbPluralizationService = new StringBidirectionalDictionary(this._irregularVerbList);
      this._knownSingluarWords = new List<string>(this._irregularPluralsList.Keys.Concat<string>((IEnumerable<string>) this._assimilatedClassicalInflectionList.Keys).Concat<string>((IEnumerable<string>) this._oSuffixList.Keys).Concat<string>((IEnumerable<string>) this._classicalInflectionList.Keys).Concat<string>((IEnumerable<string>) this._irregularVerbList.Keys).Concat<string>((IEnumerable<string>) this._uninflectiveWords).Except<string>((IEnumerable<string>) this._knownConflictingPluralList));
      this._knownPluralWords = new List<string>(this._irregularPluralsList.Values.Concat<string>((IEnumerable<string>) this._assimilatedClassicalInflectionList.Values).Concat<string>((IEnumerable<string>) this._oSuffixList.Values).Concat<string>((IEnumerable<string>) this._classicalInflectionList.Values).Concat<string>((IEnumerable<string>) this._irregularVerbList.Values).Concat<string>((IEnumerable<string>) this._uninflectiveWords));
    }

    /// <summary>
    /// Constructs a new  instance  of default pluralization service
    /// used in Entity Framework.
    /// </summary>
    /// <param name="userDictionaryEntries">
    ///     A collection of user dictionary entries to be used by this service.These inputs
    ///     can  customize the service according the user needs.
    /// </param>
    public EnglishPluralizationService(
      IEnumerable<CustomPluralizationEntry> userDictionaryEntries)
      : this()
    {
      Check.NotNull<IEnumerable<CustomPluralizationEntry>>(userDictionaryEntries, nameof (userDictionaryEntries));
      userDictionaryEntries.Each<CustomPluralizationEntry>((Action<CustomPluralizationEntry>) (entry => this._userDictionary.AddValue(entry.Singular, entry.Plural)));
    }

    /// <summary>Returns the plural form of the specified word.</summary>
    /// <returns>The plural form of the input parameter.</returns>
    /// <param name="word">The word to be made plural.</param>
    public string Pluralize(string word)
    {
      return EnglishPluralizationService.Capitalize(word, new Func<string, string>(this.InternalPluralize));
    }

    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private string InternalPluralize(string word)
    {
      if (this._userDictionary.ExistsInFirst(word))
        return this._userDictionary.GetSecondValue(word);
      if (this.IsNoOpWord(word))
        return word;
      string prefixWord;
      string suffixWord = EnglishPluralizationService.GetSuffixWord(word, out prefixWord);
      if (this.IsNoOpWord(suffixWord) || this.IsUninflective(suffixWord) || (this._knownPluralWords.Contains(suffixWord.ToLowerInvariant()) || this.IsPlural(suffixWord)))
        return prefixWord + suffixWord;
      if (this._irregularPluralsPluralizationService.ExistsInFirst(suffixWord))
        return prefixWord + this._irregularPluralsPluralizationService.GetSecondValue(suffixWord);
      string newWord;
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "man"
      }, (Func<string, string>) (s => s.Remove(s.Length - 2, 2) + "en"), this._culture, out newWord))
        return prefixWord + newWord;
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "louse",
        "mouse"
      }, (Func<string, string>) (s => s.Remove(s.Length - 4, 4) + "ice"), this._culture, out newWord))
        return prefixWord + newWord;
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "tooth"
      }, (Func<string, string>) (s => s.Remove(s.Length - 4, 4) + "eeth"), this._culture, out newWord))
        return prefixWord + newWord;
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "goose"
      }, (Func<string, string>) (s => s.Remove(s.Length - 4, 4) + "eese"), this._culture, out newWord))
        return prefixWord + newWord;
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "foot"
      }, (Func<string, string>) (s => s.Remove(s.Length - 3, 3) + "eet"), this._culture, out newWord))
        return prefixWord + newWord;
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "zoon"
      }, (Func<string, string>) (s => s.Remove(s.Length - 3, 3) + "oa"), this._culture, out newWord))
        return prefixWord + newWord;
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "cis",
        "sis",
        "xis"
      }, (Func<string, string>) (s => s.Remove(s.Length - 2, 2) + "es"), this._culture, out newWord))
        return prefixWord + newWord;
      if (this._assimilatedClassicalInflectionPluralizationService.ExistsInFirst(suffixWord))
        return prefixWord + this._assimilatedClassicalInflectionPluralizationService.GetSecondValue(suffixWord);
      if (this._classicalInflectionPluralizationService.ExistsInFirst(suffixWord))
        return prefixWord + this._classicalInflectionPluralizationService.GetSecondValue(suffixWord);
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "trix"
      }, (Func<string, string>) (s => s.Remove(s.Length - 1, 1) + "ces"), this._culture, out newWord))
        return prefixWord + newWord;
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "eau",
        "ieu"
      }, (Func<string, string>) (s => s + "x"), this._culture, out newWord))
        return prefixWord + newWord;
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "inx",
        "anx",
        "ynx"
      }, (Func<string, string>) (s => s.Remove(s.Length - 1, 1) + "ges"), this._culture, out newWord))
        return prefixWord + newWord;
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "ch",
        "sh",
        "ss"
      }, (Func<string, string>) (s => s + "es"), this._culture, out newWord))
        return prefixWord + newWord;
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "alf",
        "elf",
        "olf",
        "eaf",
        "arf"
      }, (Func<string, string>) (s =>
      {
        if (!s.EndsWith("deaf", true, this._culture))
          return s.Remove(s.Length - 1, 1) + "ves";
        return s;
      }), this._culture, out newWord))
        return prefixWord + newWord;
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "nife",
        "life",
        "wife"
      }, (Func<string, string>) (s => s.Remove(s.Length - 2, 2) + "ves"), this._culture, out newWord))
        return prefixWord + newWord;
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "ay",
        "ey",
        "iy",
        "oy",
        "uy"
      }, (Func<string, string>) (s => s + nameof (s)), this._culture, out newWord))
        return prefixWord + newWord;
      if (suffixWord.EndsWith("y", true, this._culture))
        return prefixWord + suffixWord.Remove(suffixWord.Length - 1, 1) + "ies";
      if (this._oSuffixPluralizationService.ExistsInFirst(suffixWord))
        return prefixWord + this._oSuffixPluralizationService.GetSecondValue(suffixWord);
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "ao",
        "eo",
        "io",
        "oo",
        "uo"
      }, (Func<string, string>) (s => s + nameof (s)), this._culture, out newWord))
        return prefixWord + newWord;
      if (suffixWord.EndsWith("o", true, this._culture) || suffixWord.EndsWith("x", true, this._culture))
        return prefixWord + suffixWord + "es";
      return prefixWord + suffixWord + "s";
    }

    /// <summary>Returns the singular form of the specified word.</summary>
    /// <returns>The singular form of the input parameter.</returns>
    /// <param name="word">The word to be made singular.</param>
    public string Singularize(string word)
    {
      return EnglishPluralizationService.Capitalize(word, new Func<string, string>(this.InternalSingularize));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode")]
    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    private string InternalSingularize(string word)
    {
      if (this._userDictionary.ExistsInSecond(word))
        return this._userDictionary.GetFirstValue(word);
      if (this.IsNoOpWord(word))
        return word;
      string prefixWord;
      string suffixWord = EnglishPluralizationService.GetSuffixWord(word, out prefixWord);
      if (this.IsNoOpWord(suffixWord) || this.IsUninflective(suffixWord) || this._knownSingluarWords.Contains(suffixWord.ToLowerInvariant()))
        return prefixWord + suffixWord;
      if (this._irregularVerbPluralizationService.ExistsInSecond(suffixWord))
        return prefixWord + this._irregularVerbPluralizationService.GetFirstValue(suffixWord);
      if (this._irregularPluralsPluralizationService.ExistsInSecond(suffixWord))
        return prefixWord + this._irregularPluralsPluralizationService.GetFirstValue(suffixWord);
      if (this._wordsEndingWithSisPluralizationService.ExistsInSecond(suffixWord))
        return prefixWord + this._wordsEndingWithSisPluralizationService.GetFirstValue(suffixWord);
      if (this._wordsEndingWithSePluralizationService.ExistsInSecond(suffixWord))
        return prefixWord + this._wordsEndingWithSePluralizationService.GetFirstValue(suffixWord);
      string newWord;
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "men"
      }, (Func<string, string>) (s => s.Remove(s.Length - 2, 2) + "an"), this._culture, out newWord))
        return prefixWord + newWord;
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "lice",
        "mice"
      }, (Func<string, string>) (s => s.Remove(s.Length - 3, 3) + "ouse"), this._culture, out newWord))
        return prefixWord + newWord;
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "teeth"
      }, (Func<string, string>) (s => s.Remove(s.Length - 4, 4) + "ooth"), this._culture, out newWord))
        return prefixWord + newWord;
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "geese"
      }, (Func<string, string>) (s => s.Remove(s.Length - 4, 4) + "oose"), this._culture, out newWord))
        return prefixWord + newWord;
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "feet"
      }, (Func<string, string>) (s => s.Remove(s.Length - 3, 3) + "oot"), this._culture, out newWord))
        return prefixWord + newWord;
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "zoa"
      }, (Func<string, string>) (s => s.Remove(s.Length - 2, 2) + "oon"), this._culture, out newWord))
        return prefixWord + newWord;
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "ches",
        "shes",
        "sses"
      }, (Func<string, string>) (s => s.Remove(s.Length - 2, 2)), this._culture, out newWord))
        return prefixWord + newWord;
      if (this._assimilatedClassicalInflectionPluralizationService.ExistsInSecond(suffixWord))
        return prefixWord + this._assimilatedClassicalInflectionPluralizationService.GetFirstValue(suffixWord);
      if (this._classicalInflectionPluralizationService.ExistsInSecond(suffixWord))
        return prefixWord + this._classicalInflectionPluralizationService.GetFirstValue(suffixWord);
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "trices"
      }, (Func<string, string>) (s => s.Remove(s.Length - 3, 3) + "x"), this._culture, out newWord))
        return prefixWord + newWord;
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "eaux",
        "ieux"
      }, (Func<string, string>) (s => s.Remove(s.Length - 1, 1)), this._culture, out newWord))
        return prefixWord + newWord;
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "inges",
        "anges",
        "ynges"
      }, (Func<string, string>) (s => s.Remove(s.Length - 3, 3) + "x"), this._culture, out newWord))
        return prefixWord + newWord;
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "alves",
        "elves",
        "olves",
        "eaves",
        "arves"
      }, (Func<string, string>) (s => s.Remove(s.Length - 3, 3) + "f"), this._culture, out newWord))
        return prefixWord + newWord;
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "nives",
        "lives",
        "wives"
      }, (Func<string, string>) (s => s.Remove(s.Length - 3, 3) + "fe"), this._culture, out newWord))
        return prefixWord + newWord;
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "ays",
        "eys",
        "iys",
        "oys",
        "uys"
      }, (Func<string, string>) (s => s.Remove(s.Length - 1, 1)), this._culture, out newWord))
        return prefixWord + newWord;
      if (suffixWord.EndsWith("ies", true, this._culture))
        return prefixWord + suffixWord.Remove(suffixWord.Length - 3, 3) + "y";
      if (this._oSuffixPluralizationService.ExistsInSecond(suffixWord))
        return prefixWord + this._oSuffixPluralizationService.GetFirstValue(suffixWord);
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "aos",
        "eos",
        "ios",
        "oos",
        "uos"
      }, (Func<string, string>) (s => suffixWord.Remove(suffixWord.Length - 1, 1)), this._culture, out newWord))
        return prefixWord + newWord;
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "ces"
      }, (Func<string, string>) (s => s.Remove(s.Length - 1, 1)), this._culture, out newWord))
        return prefixWord + newWord;
      if (PluralizationServiceUtil.TryInflectOnSuffixInWord(suffixWord, (IEnumerable<string>) new List<string>()
      {
        "ces",
        "ses",
        "xes"
      }, (Func<string, string>) (s => s.Remove(s.Length - 2, 2)), this._culture, out newWord))
        return prefixWord + newWord;
      if (suffixWord.EndsWith("oes", true, this._culture))
        return prefixWord + suffixWord.Remove(suffixWord.Length - 2, 2);
      if (suffixWord.EndsWith("ss", true, this._culture) || !suffixWord.EndsWith("s", true, this._culture))
        return prefixWord + suffixWord;
      return prefixWord + suffixWord.Remove(suffixWord.Length - 1, 1);
    }

    private bool IsPlural(string word)
    {
      if (this._userDictionary.ExistsInSecond(word))
        return true;
      if (this._userDictionary.ExistsInFirst(word))
        return false;
      if (this.IsUninflective(word) || this._knownPluralWords.Contains(word.ToLower(this._culture)))
        return true;
      return !this.Singularize(word).Equals(word);
    }

    private static string Capitalize(string word, Func<string, string> action)
    {
      string str = action(word);
      if (!EnglishPluralizationService.IsCapitalized(word) || str.Length == 0)
        return str;
      StringBuilder stringBuilder = new StringBuilder(str.Length);
      stringBuilder.Append(char.ToUpperInvariant(str[0]));
      stringBuilder.Append(str.Substring(1));
      return stringBuilder.ToString();
    }

    private static string GetSuffixWord(string word, out string prefixWord)
    {
      int num = word.LastIndexOf(' ');
      prefixWord = word.Substring(0, num + 1);
      return word.Substring(num + 1);
    }

    private static bool IsCapitalized(string word)
    {
      if (!string.IsNullOrEmpty(word))
        return char.IsUpper(word, 0);
      return false;
    }

    private static bool IsAlphabets(string word)
    {
      return !string.IsNullOrEmpty(word.Trim()) && word.Equals(word.Trim()) && !Regex.IsMatch(word, "[^a-zA-Z\\s]");
    }

    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    private bool IsUninflective(string word)
    {
      return PluralizationServiceUtil.DoesWordContainSuffix(word, (IEnumerable<string>) this._uninflectiveSuffixes, this._culture) || !word.ToLower(this._culture).Equals(word) && word.EndsWith("ese", false, this._culture) || ((IEnumerable<string>) this._uninflectiveWords).Contains<string>(word.ToLowerInvariant());
    }

    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    private bool IsNoOpWord(string word)
    {
      return !EnglishPluralizationService.IsAlphabets(word) || word.Length <= 1 || this._pronounList.Contains(word.ToLowerInvariant());
    }
  }
}
