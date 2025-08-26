namespace Clipo.Domain.AggregatesModel.VideoAggregate.Enums
{
    /// <summary>
    /// Representa os estados possíveis de um job de processamento de vídeo.
    /// </summary>
    public enum ProcessStatus
    {
        /// <summary>
        /// Job criado mas ainda não processado.
        /// </summary>
        Queued = 0,

        /// <summary>
        /// Em processamento no worker.
        /// </summary>
        Processing = 1,

        /// <summary>
        /// Finalizado com sucesso (frames extraídos + zip gerado).
        /// </summary>
        Done = 2,

        /// <summary>
        /// Finalizado com erro.
        /// </summary>
        Error = 3
    }
}
