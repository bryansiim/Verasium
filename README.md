# Verasium – Software de Detecção de Conteúdo Gerado por IA  

## Bryan José (2025) 

O **Verasium** é um software desenvolvido em **C# (.NET 9)** para detecção de conteúdo sintético (gerado por Inteligência Artificial).  
Ele identifica textos e imagens artificiais através da **leitura de metadados** e de **análises heurísticas** .  

---

## Status do Projeto  
**Versão Beta – em desenvolvimento ativo.**  
Esta é uma versão inicial, com funcionalidades principais já implementadas. Melhorias de desempenho, novas análises e interface gráfica serão adicionadas em versões futuras.  

---

## Funcionalidades Atuais  
- Extração e leitura de metadados (EXIF, XMP, C2PA)  
- Análise heurística para detecção de conteúdo gerado por IA  
- Geração de score de confiança baseado em múltiplos parâmetros    

---

## Próximas Atualizações  
- Interface gráfica moderna (WPF)  
- Análise visual direta de imagens, além dos metadados
- Otimização de desempenho e redução de falsos positivos  

---

## Tecnologias Utilizadas  
- **Linguagem:** C#  
- **Framework:** .NET 9  
- **APIs:** Google.GenAI, MetadataExtractor  

---

## Bibliotecas e APIs Externas  

Este software pode utilizar as seguintes bibliotecas e APIs de terceiros:  

1. **Google.GenAI**  
   - Licença: Apache 2.0  
   - Copyright (c) Google LLC  

2. **MetadataExtractor**  
   - Licença: Apache 2.0  
   - Copyright (c) Drew Noakes  

Esses serviços externos são utilizados apenas por meio de chamadas de API e **não são redistribuídos** com este software.  
Seus respectivos termos e licenças permanecem válidos e independentes do código-fonte deste projeto, que está sob **GPLv3**.  

---

## Licença  
Este projeto é licenciado sob os termos da **GNU General Public License v3.0 (GPLv3)**.  
Para mais informações, consulte o arquivo [LICENSE](./LICENSE).  
